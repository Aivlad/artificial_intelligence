using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PBH 
{
    private Element root;
    public int Count {get; protected set;}

    public PBH()
    {
        root = null;
        Count++;
    }

    public void Add(GameObject newGO)
    {
        var newElement = new Element(newGO);
        Count++;
        if (root == null)
        {
            root = newElement;
            return;
        }
        var currentElement = root;
        while(true)
        {
            if (currentElement.GetF() >= newElement.GetF() && currentElement.nextLeftElement == null)
            {
                currentElement.nextLeftElement = newElement;
                newElement.parentElement = currentElement;
                return;
            }
            else if (currentElement.GetF() < newElement.GetF() && currentElement.nextRightElement == null)
            {
                currentElement.nextRightElement = newElement;
                newElement.parentElement = currentElement;
                return;
            }
            else if (currentElement.GetF() >= newElement.GetF())
            {
                currentElement = currentElement.nextLeftElement;
            }
            else
            {
                currentElement = currentElement.nextRightElement; 
            }
        }
        
    }
    
    public GameObject GetPriorityElement()
    {
        if (root == null) return null;
        var currentElement = root;
        //слева всегда наименьшее (или равное) значение
        while (currentElement.nextLeftElement != null) currentElement = currentElement.nextLeftElement;
        //если мы в руте
        if (currentElement == root)
        {
            if (currentElement.nextRightElement != null)
            {
                root = currentElement.nextRightElement;
                currentElement.nextRightElement.parentElement = null;
                Count --;
            }
            else
            {
                root = null;
                Count = 0;
            }
            return currentElement.element;
        }
        //проверка: есть ли справа ветвь продолжения
        if (currentElement.nextRightElement != null)
        {
            currentElement.parentElement.nextLeftElement = currentElement.nextRightElement;
            currentElement.nextRightElement.parentElement = currentElement.parentElement;
            Count --;
            return currentElement.element;
        }
        //мы просто слева без ветвей
        currentElement.parentElement.nextLeftElement = null;
        Count --;
        return currentElement.element;
    }

    public GameObject Search(GameObject element)
    {
        if (root == null) return null;
        var currentElement = root;
        var searchElement = new Element(element);

        while (currentElement != null)
        {
            if (currentElement.element == searchElement.element)
            {
                return currentElement.element;
            }
            if (currentElement.GetF() < searchElement.GetF())
            {
                currentElement = currentElement.nextLeftElement;
                continue;
            }
            currentElement = currentElement.nextRightElement;
        }
        return null;
    }

    class Element
    {
        public GameObject element;
        private float priority;

        public Element nextLeftElement;
        public Element nextRightElement;
        public Element parentElement;

        public Element(GameObject element)
        {
            this.element = element;
            this.priority = element.GetComponent<NodeElement>().F;
            nextLeftElement = null;
            nextRightElement = null;
            parentElement = null;
        }

        public float GetF()
        {
            return priority;
        }

    }
}
