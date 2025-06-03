using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
public class Deep : MonoBehaviour
{
    private int orderBehind = 5; 
    private int orderInFront = -5; 
    public Renderer skeletonRenderer;
    private void OrderLayer()
    {if (GameManager.instance.Player.transform.position.z > transform.position.z)
    {skeletonRenderer.sortingOrder  = orderBehind;}else{skeletonRenderer.sortingOrder  = orderInFront;}}
    void Update(){OrderLayer();}
}