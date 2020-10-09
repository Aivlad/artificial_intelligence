using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Очередь с приоритетом в виде линейного списка. Приоритет в порядке возрастания. Значение приоритета выражается в виде F
/// </summary>
public class PriorityQueue 
{
    /// <summary>
    /// Голова очереди
    /// </summary>
    private QueueElement root;
    /// <summary>
    /// Количество элементов в списке
    /// </summary>
    public int Count {get; protected set;}

    public PriorityQueue()
    {
        root = null;
        Count = 0;
    }

    /// <summary>
    /// Добавить элемент в очередь
    /// </summary>
    public void Add(GameObject element)
    {
        QueueElement newElement = new QueueElement(element);
        
        //если у нас очередь пуста, то устанавливаем голову очереди
        if (root == null)
        {
            root = newElement;
            Count++;
            return;
        }

        //если у нового элемента F меньше чем у головы, то новый ставим как голову
        if (root.GetF() > newElement.GetF())
        {
            root.parentElement = newElement;
            newElement.nextElement = root;
            root = newElement;
            Count++;
            return;
        }

        //ищем место куда добаить элемент
        QueueElement currentElement = root.nextElement;
        while (currentElement != null)
        {
            //мы нашли место добавления
            if (currentElement.GetF() > newElement.GetF())
            {
                newElement.nextElement = currentElement;
                newElement.parentElement = currentElement.parentElement;
                currentElement.parentElement.nextElement = newElement;
                currentElement.parentElement = newElement;
                Count++;
                return;
            }
            //конец очереди
            if (currentElement.nextElement == null)
            {
                currentElement.nextElement = newElement;
                newElement.parentElement = currentElement;
                Count++;
                return;
            }
            currentElement = currentElement.nextElement;
        }
    }

    /// <summary>
    /// Получить первый элемент из очереди
    /// </summary>
    public GameObject GetPriorityElement()
    {
        QueueElement first = root;
        //очередь пуста
        if (first == null) return null;

        //у нас 1 элемент
        if (first.nextElement == null)
        {
            root = null;
            Count--;
            return first.element;
        }

        //у нас больше 1 элемента
        root = first.nextElement;
        root.parentElement = null;
        Count--;
        return first.element;
    }

    /// <summary>
    /// Ищет элемент в списке
    /// </summary>
    /// <param name="element">Объект поиска</param>
    /// <returns>Возвращает объект если он найден в списке, иначе null</returns>
    public GameObject Search(GameObject element)
    {
        QueueElement currentElement = root;
        while (currentElement != null)
        {
            if (currentElement.element.Equals(element))
                return element;
            currentElement = currentElement.nextElement;
        }
        return null;
    }

    /// <summary>
    /// Элемент очереди
    /// </summary>
    private class QueueElement
    {
        /// <summary>
        /// Элемент в очереди
        /// </summary>
        public GameObject element {get;}
        /// <summary>
        /// Приоритет объекта (выражается  в виде значения F)
        /// </summary>
        /// <value></value>
        public float priority {get;}

        /// <summary>
        /// Ссылка на предыдущий элемент
        /// </summary>
        public QueueElement parentElement {get; set;}

        /// <summary>
        /// Ссылка на следующий элемент
        /// </summary>
        public QueueElement nextElement {get; set;}

        public QueueElement(GameObject element)
        {
            this.element = element;
            this.priority = element.GetComponent<NodeElement>().F;
            parentElement = null;
            nextElement = null;
        }

        /// <summary>
        /// Получить значение F элемента очереди
        /// </summary>
        public float GetF()
        {
            return priority;
        }
    }
}
