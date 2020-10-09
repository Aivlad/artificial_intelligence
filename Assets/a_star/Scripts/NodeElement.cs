using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeElement : MonoBehaviour
{
    /// <summary>
    /// Является ли объект препятствием или нет
    /// </summary>
    public bool isLet = false;
    /// <summary>
    /// Посещали ли мы этот узел
    /// </summary>
    public bool isVisit = false;

    /// <summary>
    /// Величина, вычисляемая по формуле: F = H + G
    /// </summary>
    public float F {get; set;}
    /// <summary>
    /// Примерное количество энергии, затрачиваемое на передвижение от текущей клетки до целевой клетки B
    /// </summary>
    public float H {get; set;}
    /// <summary>
    /// Энергия, затрачиваемая на передвижение из стартовой клетки A в текущую рассматриваемую клетку, следуя найденному пути к этой клетке
    /// </summary>
    public float G {get; set;}
    /// <summary>
    /// Ссыдка на родителя
    /// </summary>
    public GameObject parent {get; set;}

    // void OnTriggerEnter(Collider colleder)
    // {
    //     LightOnLet();   //красим как препятствие
    //     isLet = true;
    // }

    // void OnTriggerExit(Collider collider)
    // {
    //     LightOffLet();  //снимаем окраску
    //     isLet = false;
    // }

    void OnCollisionEnter(Collision coll)
    {
        if (coll.gameObject.tag == "let")
        {
            LightOnLet();   //красим как препятствие
            isLet = true;
            // coll.gameObject.GetComponent<Rigidbody>().isKinematic = false;   //один из способов убрать отскок
        }        
    }

    void OnCollisionExit(Collision coll)
    {
        if (coll.gameObject.tag == "let")
        {
            LightOffLet();  //снимаем окраску
            isLet = false;
            // coll.gameObject.GetComponent<Rigidbody>().isKinematic = true;  //включаем обратно
        }
    }

    /// <summary>
    /// Отметить узел, который посетили
    /// </summary>
    public void LightOnVisited()
    {
        GetComponent<MeshRenderer>().material.color = Color.red;
        isVisit = true;
    }
    /// <summary>
    /// Снять отметку посещения с узла
    /// </summary>
    public void LightOffVisited()
    {
        GetComponent<MeshRenderer>().material.color = Color.white;
        isVisit = false;
    }

    /// <summary>
    /// Отметить узел как препятствие
    /// </summary>
    public void LightOnLet()
        => GetComponent<MeshRenderer>().material.color = Color.black;

    /// <summary>
    /// Снять отметку препятствия с узла
    /// </summary>
    public void LightOffLet()
        => GetComponent<MeshRenderer>().material.color = Color.white;


    /// <summary>
    /// Является ли узел потенциальным элементом пути или нет
    /// </summary>
    /// <returns>True - узел можно использовать как элемент пути, иначе нельзя</returns>
    public bool IsConsiderNode()
        => !isLet && !isVisit;  


    void Start()
    {
        isLet = false;
        LightOffLet();
        LightOffVisited();    
    }
}
