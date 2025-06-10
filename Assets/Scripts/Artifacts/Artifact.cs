using UnityEngine;
using System;

/// <summary>
/// Описывает один артефакт: имя, описание, редкость, цвет рамки, иконку и действие (эффект) при подборе.
/// </summary>
[Serializable]
public class Artifact
{
    public enum Rarity { Common, Rare, Epic }

    public string artifactName;        // Название
    [TextArea] public string description;  // Описание (подробное)
    public Rarity rarity;             // Редкость
    public Color rarityColor;         // Цвет рамки под редкость
    public Sprite icon;               // Иконка артефакта
    public Action<PlayerController> ApplyEffect; // Делегат для применения эффекта к игроку

    // Конструктор для создания вручную
    public Artifact(
        string name,
        string desc,
        Rarity r,
        Color color,
        Sprite iconSprite,
        Action<PlayerController> effect)
    {
        artifactName  = name;
        description   = desc;
        rarity        = r;
        rarityColor   = color;
        icon          = iconSprite;
        ApplyEffect   = effect;
    }
}
