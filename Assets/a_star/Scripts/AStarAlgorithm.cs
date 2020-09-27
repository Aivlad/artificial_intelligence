using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarAlgorithm : MonoBehaviour
{
    [Header("Node-start")]
    public int nodeStartX;
    public int nodeStartY;

    [Header("Node-end")]
    public int nodeEndX;
    public int nodeEndY;

    [Header("Additional settings")]
    private bool isReply;
    private GameObject[,] nodes;
    private int countNodes;

    void Start()
    {
        isReply = true;
    }

    void Update()
    {
        if (isReply)
        {
            nodes = gameObject.GetComponent<GenerationRules>().nodes;
            countNodes = nodes.GetLength(0);
            Action();
            isReply = false;
        }
    }

    /// <summary>
    /// Выполнение алгоритма A*
    /// </summary>
    public void Action()
    {
        //доп.условие: NodeStart = NodeEnd
        if (nodeStartX == nodeEndX && nodeStartY == nodeEndY)
        {
            Debug.Log("Стартовый и конечный узлы - это один и тот же узел");
            return;
        }
        //доп.условие: некорректные данные
        if (nodeStartX >= countNodes || nodeStartX < 0
            || nodeEndX >= countNodes || nodeEndX < 0
            || nodeStartY >= countNodes || nodeStartY < 0
            || nodeEndY >= countNodes || nodeEndY < 0)
        {
            Debug.Log("Некорректные данные: выход за границы поля");
            return;
        }

        //открытый список для нодов, ожидающих рассмотрения
        PriorityQueue openList = new PriorityQueue();
        //закоытый список для нодов, которые рассмотрели
        List<GameObject> closedList = new List<GameObject>();

        //выносим отдельно конечный узел
        GameObject endNode = GetComponent<GenerationRules>().nodes[nodeEndY, nodeEndX];

        //помечаем стартовый узел
        GameObject startNode = GetComponent<GenerationRules>().nodes[nodeStartY, nodeStartX];
        startNode.GetComponent<NodeElement>().LightOnVisited();
        //обнуляеи значения стартового узла
        SetParameters(startNode, 0, 0);

        //добавление стартовой клетки в открытый список
        openList.Add(startNode);
        while (true)
        {
            //достаем из открытого списка
            GameObject currentObject = openList.GetFirstElement();
            //кладем в закрытый
            closedList.Add(currentObject);

            //определение координат текущего узла
            var indexI = 0;
            var indexJ = 0;
            for (var i = 0; i < countNodes; i++)
            {
                var exit = false;
                for (var j = 0; j < countNodes; j++)
                    if (nodes[i, j].Equals(currentObject))
                    {
                        indexI = i;
                        indexJ = j;
                        exit = true;
                        break;
                    }
                if (exit) break;
            }
            //определяем координаты соседей (их может быть до 8)
            for (var i = -1; i <= 1; i++)
            {
                for (var j = -1; j <= 1; j++)
                {
                    //исключаем текущую клетку
                    if (i == 0 && j == 0) continue;
                    //начинаем проверку условий:
                    //1) Если клетка непроходима или находится в закрытом списке, игнорируем её.
                    if (GetObject(indexI, indexJ, i, j) != null && !closedList.Contains(nodes[indexI + i, indexJ + j]))
                    {
                        //Если клетка не в открытом списке, то добавляем её в открытый список, 
                        //при этом рассчитываем для неё значения G, H и F, и также устанавливаем ссылку родителя на текущую клетку.
                        GameObject searchElement = openList.Search(nodes[indexI + i, indexJ + j]);
                        if (searchElement == null)
                        {
                            //если рассматриваемый узел от текущего лежит по диагонале, то коэффициент g = +14 иначе +10
                            var g = currentObject.GetComponent<NodeElement>().G;
                            if (i == 0 || j == 0) g += 10;
                            else g += 14;
                            //h рассчитывается по методом Манхэттена
                            var h = (Mathf.Abs((indexI + i) - nodeEndY) + Mathf.Abs((indexJ + j) - nodeEndX)) * 10;
                            //установка параметров
                            SetParameters(nodes[indexI + i, indexJ + j], h, g, currentObject);
                            openList.Add(nodes[indexI + i, indexJ + j]);
                        }
                        else
                        //Если клетка находится в открытом списке, то сравниваем её значение G со значением G таким, что если бы к ней пришли через текущую клетку. 
                        //Если сохранённое в проверяемой клетке значение G больше нового, то меняем её значение G на новое, 
                        //пересчитываем её значение F и изменяем указатель на родителя так, чтобы она указывала на текущую клетку.
                        {
                            var oldG = searchElement.GetComponent<NodeElement>().G;
                            //если рассматриваемый узел от текущего лежит по диагонале, то коэффициент g = +14 иначе +10
                            var newG = currentObject.GetComponent<NodeElement>().G;
                            if (i == 0 || j == 0) newG += 10;
                            else newG += 14;
                            //если новое значение лучше (меньше), то меняем параметры найденного узла, иначе не трогаем ничего
                            if (newG < oldG) SetParameters(searchElement, -1, newG, currentObject);
                        }
                    }
                }
            }

            //условие выхода: в открытый список добавили целевую клетку
            if (openList.Search(endNode) != null) break;
            
            //условия выхода: открытый список пуст
            if (openList.Count == 0)
            {
                Debug.Log("Путь не был найден");
                return;
            }
        }

        //отрисовываем найденный путь
        GameObject currentNode = endNode;
        while (currentNode.GetComponent<NodeElement>().parent != null)
        {
            currentNode.GetComponent<NodeElement>().LightOnVisited();
            currentNode = currentNode.GetComponent<NodeElement>().parent;
        }
        Debug.Log("Путь найден");
        gameObject.GetComponent<GenerationRules>().ResetCooldown();
    }

    /// <summary>
    /// Установить параметры для указанного узла
    /// </summary>
    /// <param name="node">Сам узел</param>
    /// <param name="h">Примерное количество энергии, затрачиваемое на передвижение от текущей клетки до целевой клетки B. Если не требуется менять значение, то отправлять в параметр -1</param>
    /// <param name="g">Энергия, затрачиваемая на передвижение из стартовой клетки A в текущую рассматриваемую клетку, следуя найденному пути к этой клетке</param>
    /// <param name="parent">Родительский узел (тот из которого пришли)</param>
    private void SetParameters(GameObject node, int h, int g, GameObject parent = null)
    {
        if (h != -1) node.GetComponent<NodeElement>().H = h;
        node.GetComponent<NodeElement>().G = g;
        node.GetComponent<NodeElement>().F = h +  g;
        node.GetComponent<NodeElement>().parent = parent;
    }

    /// <summary>
    /// Получить объект из матрицы nodes по его индексам: index i = currentI + plusI, index j = currentJ + plusJ
    /// </summary>
    /// <returns>Если узел можно рассмотреть, то вернется объект, иначе null</returns>
    private GameObject GetObject(int currentI, int currentJ, int plusI, int plusJ)
    {
        var newI = currentI + plusI;
        var newJ = currentJ + plusJ;
        //проверка на выход за границы массива
        if (newI < 0 || newJ < 0 || newI >= countNodes || newJ >= countNodes)
            return null;
        //возврат найденного объекта, если клетка проходима
        return !nodes[newI, newJ].GetComponent<NodeElement>().isLet? nodes[newI, newJ] : null;
    }
    
}
