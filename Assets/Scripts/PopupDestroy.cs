using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupDestroy : MonoBehaviour
{
    [Range(0, 3)]
    public int star = 3;

    [SerializeField]
    private Image star1, star2, star3;
    void Start()
    {

    }

    void Update()
    {
        if (star == 0)
        {
            star1.color = new Color(1f, 1f, 1f, 0.4f);
            star2.color = new Color(1f, 1f, 1f, 0.4f);
            star3.color = new Color(1f, 1f, 1f, 0.4f);
        }
        else if (star == 1)
        {
            star1.color = new Color(1f, 1f, 1f, 1f);
            star2.color = new Color(1f, 1f, 1f, 0.4f);
            star3.color = new Color(1f, 1f, 1f, 0.4f);

        }
        else if (star == 2)
        {
            star1.color = new Color(1f, 1f, 1f, 1f);
            star2.color = new Color(1f, 1f, 1f, 1f);
            star3.color = new Color(1f, 1f, 1f, 0.4f);
        }
        else
        {
            star1.color = new Color(1f, 1f, 1f, 1f);
            star2.color = new Color(1f, 1f, 1f, 1f);
            star3.color = new Color(1f, 1f, 1f, 1f);
        }
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
