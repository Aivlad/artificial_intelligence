using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerationRules : MonoBehaviour
{
    [Header("Floor Generation Settings")]
    public GameObject prefabFloor;
    public int sizeFloor;

    [Header("Obstacle Generation Settings")]
    public GameObject prefabLet;
    private GameObject parentObstacle;
    [Range(1,3)] public int mode = 1;

    [Header("Nodes Generation Settings")]
    public GameObject prefabNode;
    public int sizeNode = 10;
    public Vector3 firstNodeCoordinates;
    private int countNodes;
    public GameObject[,] nodes = null;
    private GameObject parentForNodes; 

    [Header("Addition Settings")]
    private bool isCooldown;


    /// <summary>
    /// Генерация узлов
    /// </summary>
    private void CreateNodes()
    {
        //рассчитуем количество нодов (чтобы поле было полностью заполнено)
        countNodes = sizeFloor / 2;
        nodes = new GameObject[countNodes, countNodes];

        //размер нода
        prefabNode.GetComponent<Transform>().localScale = new Vector3(sizeNode, sizeNode, sizeNode);

        //расстояние между нодами
        var distanceToNexntNode = sizeNode * 2;

        //определяемкоординаты первого узла
        var x = firstNodeCoordinates.x;
        var y = firstNodeCoordinates.y;
        var z = firstNodeCoordinates.z;
        for(var i = 0; i < countNodes; i++)
        {
            for (var j = 0; j < countNodes; j++)
            {
                nodes[i, j] = Spawn(prefabNode, x, y, z);
                nodes[i, j].transform.SetParent(parentForNodes.transform);  //чтобы не захламлять иерархию сцены
                x += distanceToNexntNode;   
            }
            z += distanceToNexntNode;
            x = firstNodeCoordinates.x;
        }
    }

    /// <summary>
    /// Генерация пола
    /// </summary>
    private void CreateFloor()
    {
        //координата, которая определяет левый нижний угол
        float coordinate = sizeFloor / 2 * 10;
        //спавн и настройка
        prefabFloor.transform.localScale = new Vector3(sizeFloor, sizeFloor, sizeFloor);
        Spawn(prefabFloor, coordinate, 0f, coordinate);
    }
    
    /// <summary>
    /// Cоздание префаба объекта на сцене
    /// </summary>
    /// <param name="prefab">Префаб</param>
    /// <param name="x">Координата Х</param>
    /// <param name="y">Координата Y</param>
    /// <param name="z">Координата Х</param>
    /// <param name="dx">Угол поворота по dx. По умолчанию 0</param>
    /// <param name="dy">Угол поворота по dy. По умолчанию 0</param>
    /// <param name="dz">Угол поворота по dz. По умолчанию 0</param>
    /// <returns>Созданный объект</returns>
	private GameObject Spawn(GameObject prefab, float x, float y, float z, float dx = 0f, float dy = 0f, float dz = 0f)
	{
		GameObject spawnObject = GameObject.Instantiate(prefab);
		spawnObject.transform.position = new Vector3(x, y, z);
		spawnObject.transform.rotation = Quaternion.Euler(dx, dy, dz);
        return spawnObject;
	}

    /// <summary>
    /// Создание узловых препятствий
    /// </summary>
    private void CreateLet()
    {
        if (mode == 1) DrawRoom(true);
        else if (mode == 2) DrawRoom(false);        
        else
        {
            //TODO: здесь можно прописывать свои расположения препятствий
        }        
    }

    /// <summary>
    /// Загатовка комната для тестирования
    /// </summary>
    /// <param name="isPassage">Оставить вход в комнату (true) или нет (false)</param>
    private void DrawRoom(bool isPassage)
    {
        int count = 19;
            var distance = sizeNode * 2;
            var x1 = 70;
            var x2 = distance * (count - 1) + x1;
            var y1 = 70;        
            GameObject obstacle;    
            for (var i = 0; i < count; i++)
            {
                obstacle = Spawn(prefabLet, x1, 100, y1);     
                obstacle.transform.SetParent(parentObstacle.transform);
                obstacle = Spawn(prefabLet, x2, 100, y1);     
                obstacle.transform.SetParent(parentObstacle.transform);
                y1 += distance;
            }
            x1 = 90;
            y1 = 70;
            var y2 = distance * (count - 1) + y1;
            for (var i = 0; i < count - 2; i++)
            {
                obstacle = Spawn(prefabLet, x1, 100, y1);     
                obstacle.transform.SetParent(parentObstacle.transform);
                if (isPassage && x1 == 210) {
                    x1 += distance;
                    continue;
                }
                obstacle = Spawn(prefabLet, x1, 100, y2);     
                obstacle.transform.SetParent(parentObstacle.transform);
                x1 += distance;
            }
    }

    /// <summary>
    /// Снять КД с клавиши Space
    /// </summary>
    public void ResetCooldown()
    {
        isCooldown = false;
    }

    /// <summary>
    /// Сбросить посещение всех узлов
    /// </summary>
    private void ResetLightNodes()
    {
        foreach(GameObject node in nodes)
            if (node.GetComponent<NodeElement>().isLet)
                node.GetComponent<NodeElement>().LightOnLet();
            else
                node.GetComponent<NodeElement>().LightOffVisited();
    }

    void Start()
    {
        isCooldown = false;
        parentForNodes = GameObject.Find("Nodes");
        parentObstacle = GameObject.Find("Obstacle");
        CreateFloor();
        CreateNodes();
        CreateLet();
    }

    void Update()
    {
        if (!isCooldown && Input.GetKeyDown(KeyCode.Space))
        {
            isCooldown = true;
            ResetLightNodes();
            gameObject.GetComponent<AStarAlgorithm>().Action();
        }
    }
}
