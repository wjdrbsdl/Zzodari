using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectZoneColorController : MonoBehaviour
{
    public Color myTurnColor;
    public Color otherTurnColor = Color.black;
    public SpriteRenderer m_sprite;

    private void Start()
    {
        myTurnColor = m_sprite.color;
    }

    public void ChangeColor(bool _isMyTurn)
    {
        Color color;
        if (_isMyTurn)
        {
            color = myTurnColor;
        }
        else
        {
            color = otherTurnColor;
        }
        m_sprite.color = color;
    }
}
