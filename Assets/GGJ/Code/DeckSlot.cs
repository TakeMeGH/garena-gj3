using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DeckSlot : MonoBehaviour, IDropHandler
{
    public bool isDeck = true; // true = deck slot, false = inventory slot
    public int index = 0; // deck slot index (0-15)
    public Draggable occupied; // for deck slot: single token
    public List<Draggable> occupieds = new List<Draggable>(); // for inventory: multiple tokens
    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        if (dropped == null) return;
        Draggable draggable = dropped.GetComponent<Draggable>();
        if (draggable == null) return;

        Debug.Log($"{dropped.name} dropped on {(isDeck ? "DeckSlot" : "InventorySlot")}, index {index}");

        // Remove from previous slot
        DeckSlot prevSlot = draggable.parentAfterDrag.GetComponent<DeckSlot>();
        if (prevSlot != null)
        {
            if (prevSlot.isDeck)
                prevSlot.occupied = null;
            else
                prevSlot.occupieds.Remove(draggable);
        }

        // Deck slot: only one token
        if (isDeck)
        {
            if (occupied == null)
            {
                draggable.parentAfterDrag = transform;
                occupied = draggable;
            }
            else
            {
                Debug.Log("Deck slot already occupied.");
                // Optionally: return token to previous slot or reject
            }
        }
        // Inventory slot: multiple tokens
        else
        {
            draggable.parentAfterDrag = transform;
            occupieds.Add(draggable);
        }
        // Optionally: update visuals/UI here
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Optionally: initialize slot visuals
    }

    // Update is called once per frame
    void Update()
    {
        // Optionally: update slot visuals
    }
}
