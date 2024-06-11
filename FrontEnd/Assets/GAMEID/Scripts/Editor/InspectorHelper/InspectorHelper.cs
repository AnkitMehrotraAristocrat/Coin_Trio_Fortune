#region  Using

using UnityEditor;
using UnityEngine;

#endregion

/// <summary>
/// Methods that help with creating editor GUIs.
/// </summary>
public static partial class InspectorHelper
{
    /// <summary>
    /// Depth to tab in items for nested levels.
    /// </summary>
    public const int TabDepth = 25;

    /// <summary>
    /// Delegate describing the required GUI method for a field type.
    /// </summary>
    /// <typeparam name="T">Value Type of GUI Element.</typeparam>
    /// <param name="value">Display Value.</param>
    /// <param name="options">
    /// An optional list of layout options that specify extra layout properties.
    /// Any values passed in here will override settings defined by the style.
    /// </param>
    /// <returns>Current value from the User.</returns>
    public delegate T FieldGUI<T>(T value, params GUILayoutOption[] options);

    #region Delegates

    /// <summary>
    /// Delegate that is used to add a <see cref="UnityEngine.Object"/> to a collection.
    /// </summary>
    /// <param name="item">The <see cref="UnityEngine.Object"/> that should be added to the collection.</param>
    /// <returns>Flag indicating if the add was successful.</returns>
    public delegate bool CollectionAddObject(Object item);

    /// <summary>
    /// Delegate that is used to get the number of items in a collection.
    /// </summary>
    /// <returns>Number of items in the collection.</returns>
    public delegate int CollectionGetCount();

    /// <summary>
    /// Delegate that is used to remove an item at a specific index.
    /// </summary>
    /// <param name="index">Index of the item to remove from the collection.</param>
    public delegate void CollectionRemoveAt(int index);

    /// <summary>
    /// Delegate that is used to draw a GUI filed for a specific item in a collection.
    /// </summary>
    /// <param name="caption">Caption that should be set for the GUI field.</param>
    /// <param name="index">Index of the collection item to create the GUI field for.</param>
    public delegate void CollectionDrawGuiField(string caption, int index);

    /// <summary>
    /// Delegate to get a the item from a collection at a specific index.
    /// </summary>
    /// <typeparam name="T">Type of the item that will be returned from the collection.</typeparam>
    /// <param name="index">Index of the item to get.</param>
    /// <returns>Item at the specified index of the collection.</returns>
    public delegate T CollectionGetItemAt<out T>(int index);

    /// <summary>
    /// Delegate to set the item at a specific index in a collection.
    /// </summary>
    /// <typeparam name="T">Type of the item that will be added.</typeparam>
    /// <param name="item">The item that should be added.</param>
    /// <param name="index">Index of the location to add the item.</param>
    public delegate void CollectionSetItemAt<in T>(T item, int index);

    #endregion

    #region List

    /// <summary>
    /// Create a list of specific typed objects that can be changed by the user.
    /// </summary>
    /// <typeparam name="T">Type of object that the list will contain.</typeparam>
    /// <param name="label">Label for the list.</param>
    /// <param name="toolTip">Tool tip information for users.</param>
    /// <param name="foldout">Flag indicating if the list is expanded(true) or collapsed.</param>
    /// <param name="allowEdit">Flag indicating if this list is editable by the user.</param>
    /// <param name="nestLevel">Level to tab in the list items.</param>
    /// <param name="count">Method to get the number of items in the list.</param>
    /// <param name="addObject">Method that will be used to add an <see cref="Object"/> to the collection.</param>
    /// <param name="remove">Method to remove an item at a specific index.</param>
    /// <param name="getItem">Method to get an item from the collection.</param>
    /// <param name="setItem">Method to set an item in the collection.</param>
    /// <param name="drawGui">Method to draw an editor GUI element for the item in the collection.</param>
    /// <returns>Flag indicating if the list is folded out or not.</returns>
    public static bool TypedList<T>(string label, string toolTip, bool foldout, bool allowEdit, int nestLevel,
                                    CollectionGetCount count, CollectionAddObject addObject,
                                    CollectionRemoveAt remove,
                                    CollectionGetItemAt<T> getItem, CollectionSetItemAt<T> setItem,
                                    CollectionDrawGuiField drawGui)
    {
        BeginHorizontal(nestLevel);

        var expanded = EditorGUILayout.Foldout(foldout, new GUIContent(label, toolTip));

        EndHorizontal();

        if(allowEdit && HandleTypedDragDrop(addObject))
        {
            return expanded;
        }

        if(expanded)
        {
            DrawTypedListItems(nestLevel + 1, allowEdit, count, addObject, remove, getItem, setItem, drawGui);
        }

        return expanded;
    }

    /// <summary>
    /// Handle a drag and drop event from the GUI.
    /// </summary>
    /// <param name="addObject">Method that will be used to add an <see cref="Object"/> to the collection.</param>
    /// <returns>Flag indicating if the drag drop was handled.</returns>
    public static bool HandleTypedDragDrop(CollectionAddObject addObject)
    {
        var droppedObjects = CheckForDragDrop();

        var acceptDrop = false;
        if(IsDragDropLocation() && droppedObjects != null)
        {
            foreach(var droppedObject in droppedObjects)
            {
                acceptDrop |= addObject(droppedObject);
                ConsumeDragDrop(Event.current.type, acceptDrop);
            }
        }
        return acceptDrop;
    }

    #region DrawTypedListItems

    /// <summary>
    /// Create a list of items in the editor that cannot be edited.
    /// </summary>
    /// <typeparam name="T">Type of object that will be used for this list.</typeparam>
    /// <param name="nestLevel">Number of levels to tab in the list.</param>
    /// <param name="showSize">Flag indicating if the list size field should be shown.</param>
    /// <param name="count">Method to get the number of items in the list.</param>
    /// <param name="drawGui">Method to draw an editor GUI element for the item in the collection.</param>
    public static void DrawTypedListItems(int nestLevel, bool showSize, CollectionGetCount count,
                                            CollectionDrawGuiField drawGui)
    {
        DrawTypedListItems<Object>(nestLevel,
                                    false, // Allow edit
                                    showSize,
                                    false, // show up down buttons
                                    null, // caption label
                                    null, // data label
                                    count,
                                    item => false, // add
                                    index => { }, // remove
                                    index => null, // get
                                    (item, index) => { }, // Set
                                    drawGui);
    }

    /// <summary>
    /// Create a list of items in the editor.
    /// </summary>
    /// <typeparam name="T">Type of object that will be used for this list.</typeparam>
    /// <param name="nestLevel">Number of levels to tab in the list.</param>
    /// <param name="allowEdit">Flag indicating if this list is modifiable by the user.</param>
    /// <param name="count">Method to get the number of items in the list.</param>
    /// <param name="add">Method to add an item to the collection.</param>
    /// <param name="remove">Method to remove an item at a specific index.</param>
    /// <param name="getItem">Method to get an item from the collection.</param>
    /// <param name="setItem">Method to set an item in the collection.</param>
    /// <param name="drawGui">Method to draw an editor GUI element for the item in the collection.</param>
    public static void DrawTypedListItems<T>(int nestLevel, bool allowEdit, CollectionGetCount count,
                                                CollectionAddObject add,
                                                CollectionRemoveAt remove, CollectionGetItemAt<T> getItem,
                                                CollectionSetItemAt<T> setItem, CollectionDrawGuiField drawGui)
    {
        DrawTypedListItems(nestLevel, allowEdit, true, false, count, add, remove, getItem, setItem, drawGui);
    }

    /// <summary>
    /// Create a list of items in the editor.
    /// </summary>
    /// <typeparam name="T">Type of object that will be used for this list.</typeparam>
    /// <param name="nestLevel">Number of levels to tab in the list.</param>
    /// <param name="allowEdit">Flag indicating if this list is modifiable by the user.</param>
    /// <param name="showSize">Flag indicating if the list size field should be shown.</param>
    /// <param name="addButton">Flag indicating if a button should be displayed to add new items to the list.</param>
    /// <param name="count">Method to get the number of items in the list.</param>
    /// <param name="add">Method to add an item to the collection.</param>
    /// <param name="remove">Method to remove an item at a specific index.</param>
    /// <param name="getItem">Method to get an item from the collection.</param>
    /// <param name="setItem">Method to set an item in the collection.</param>
    /// <param name="drawGui">Method to draw an editor GUI element for the item in the collection.</param>
    public static void DrawTypedListItems<T>(
        int nestLevel, bool allowEdit,
        bool showSize, bool addButton,
        CollectionGetCount count,
        CollectionAddObject add,
        CollectionRemoveAt remove, CollectionGetItemAt<T> getItem,
        CollectionSetItemAt<T> setItem, CollectionDrawGuiField drawGui)
    {
        DrawTypedListItems(nestLevel, allowEdit, showSize, addButton, null, null,
                            count, add, remove, getItem, setItem, drawGui);
    }

    /// <summary>
    /// Create a list of items in the editor.
    /// </summary>
    /// <typeparam name="T">Type of object that will be used for this list.</typeparam>
    /// <param name="nestLevel">Number of levels to tab in the list.</param>
    /// <param name="allowEdit">Flag indicating if this list is modifiable by the user.</param>
    /// <param name="showSize">Flag indicating if the list size field should be shown.</param>
    /// <param name="addButton">Flag indicating if a button should be displayed to add new items to the list.</param>
    /// <param name="captionLabel">Label that will be displayed above the caption column of the list display.</param>
    /// <param name="dataLabel">Label that will be displayed above the data column of the list display.</param>
    /// <param name="count">Method to get the number of items in the list.</param>
    /// <param name="add">Method to add an item to the collection.</param>
    /// <param name="remove">Method to remove an item at a specific index.</param>
    /// <param name="getItem">Method to get an item from the collection.</param>
    /// <param name="setItem">Method to set an item in the collection.</param>
    /// <param name="drawGui">Method to draw an editor GUI element for the item in the collection.</param>
    public static void DrawTypedListItems<T>(
        int nestLevel, bool allowEdit,
        bool showSize, bool addButton,
        string captionLabel, string dataLabel,
        CollectionGetCount count,
        CollectionAddObject add,
        CollectionRemoveAt remove, CollectionGetItemAt<T> getItem,
        CollectionSetItemAt<T> setItem, CollectionDrawGuiField drawGui)
    {
        DrawTypedListItems(nestLevel, allowEdit, showSize, addButton, true, captionLabel, dataLabel, count, add,
                            remove, getItem, setItem, drawGui);
    }

    /// <summary>
    /// Create a list of items in the editor.
    /// </summary>
    /// <typeparam name="T">Type of object that will be used for this list.</typeparam>
    /// <param name="nestLevel">Number of levels to tab in the list.</param>
    /// <param name="allowEdit">Flag indicating if this list is modifiable by the user.</param>
    /// <param name="showSize">Flag indicating if the list size field should be shown.</param>
    /// <param name="addButton">Flag indicating if a button should be displayed to add new items to the list.</param>
    /// <param name="showUpDownButtons">Flag indicating if the buttons to move elements in the list should be shown.</param>
    /// <param name="captionLabel">Label that will be displayed above the caption column of the list display.</param>
    /// <param name="dataLabel">Label that will be displayed above the data column of the list display.</param>
    /// <param name="count">Method to get the number of items in the list.</param>
    /// <param name="add">Method to add an item to the collection.</param>
    /// <param name="remove">Method to remove an item at a specific index.</param>
    /// <param name="getItem">Method to get an item from the collection.</param>
    /// <param name="setItem">Method to set an item in the collection.</param>
    /// <param name="drawGui">Method to draw an editor GUI element for the item in the collection.</param>
    public static void DrawTypedListItems<T>(
        int nestLevel, bool allowEdit,
        bool showSize, bool addButton,
        bool showUpDownButtons,
        string captionLabel, string dataLabel,
        CollectionGetCount count,
        CollectionAddObject add,
        CollectionRemoveAt remove, CollectionGetItemAt<T> getItem,
        CollectionSetItemAt<T> setItem, CollectionDrawGuiField drawGui)
    {
        BeginHorizontal(nestLevel);

        EditorGUILayout.BeginVertical();

        var collectionCount = count();
        var desiredSize = collectionCount;
        if(showSize)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Size:", GUILayout.MaxWidth(30));
            desiredSize = EditorGUILayout.IntField(collectionCount, GUILayout.MaxWidth(100));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();
        }
        if(allowEdit)
        {
            //resize list to match Size.
            if(desiredSize >= 0 && desiredSize < collectionCount)
            {
                var target = collectionCount - desiredSize;
                for(var removeItemIndex = (collectionCount - 1); removeItemIndex > target; removeItemIndex--)
                {
                    remove(removeItemIndex);
                }
            }
            else if(desiredSize > collectionCount)
            {
                var target = desiredSize - collectionCount;
                for(var addItemCount = 0; addItemCount < target; addItemCount++)
                {
                    add(null);
                }
            }
        }

        EditorGUILayout.BeginHorizontal();
        if(!string.IsNullOrEmpty(captionLabel))
        {
            EditorGUILayout.LabelField(captionLabel);
        }
        if(!string.IsNullOrEmpty(dataLabel))
        {
            EditorGUILayout.LabelField(dataLabel);
        }
        EditorGUILayout.EndHorizontal();

        //Draw each element.
        for(var index = 0; index < count(); index++)
        {
            var caption = "Element " + index;
            EditorGUILayout.BeginHorizontal();

            drawGui(caption, index);

            if(allowEdit)
            {
                if(GUILayout.Button("-", GUILayout.Width(20)))
                {
                    remove(index);
                }
                if(showUpDownButtons)
                {
                    if(index > 0)
                    {
                        if(GUILayout.Button("Up", GUILayout.Width(27)))
                        {
                            var previous = getItem(index - 1);
                            setItem(getItem(index), index - 1);
                            setItem(previous, index);
                        }
                    }
                    else
                    {
                        GUILayout.Space(31);
                    }
                    if(index < (count() - 1))
                    {
                        if(GUILayout.Button("Dn", GUILayout.Width(28)))
                        {
                            var next = getItem(index + 1);
                            setItem(getItem(index), index + 1);
                            setItem(next, index);
                        }
                    }
                    else
                    {
                        GUILayout.Space(32);
                    }
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        if(allowEdit && addButton)
        {
            EditorGUILayout.Separator();
            if(Button("Add Item", "Select this to add an empty item to the list."))
            {
                add(null);
            }
        }

        EditorGUILayout.EndVertical();

        EndHorizontal();
    }

    #endregion

    #endregion

    /// <summary>
    /// Begin a horizontal GUI block for the editor.
    /// </summary>
    /// <param name="nestLevel">Amount to indent this horizontal block.</param>
    public static void BeginHorizontal(int nestLevel)
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(TabDepth * nestLevel);
    }

    /// <summary>
    /// End a horizontal GUI block for the editor.
    /// </summary>
    public static void EndHorizontal()
    {
        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// Draw a nested field type GUI element with a label and tool tip.
    /// </summary>
    /// <typeparam name="T">Value Type of GUI Element.</typeparam>
    /// <param name="value">Display Value.</param>
    /// <param name="label">Label for the field.</param>
    /// <param name="toolTip">Tool tip information for users.</param>
    /// <param name="nestLevel">Number of levels to tab in this GUI element.</param>
    /// <param name="guiMethod">GUI Method that draws the correct GUI element.</param>
    /// <param name="options">
    /// An optional list of layout options that specify extra layouting properties.
    /// Any values passed in here will override settings defined by the style.
    /// </param>
    /// <returns>Current value of the field.</returns>
    public static T Field<T>(T value, string label, string toolTip, int nestLevel,
                                FieldGUI<T> guiMethod, params GUILayoutOption[] options)
    {
        BeginHorizontal(nestLevel);

        FieldLabel(label, toolTip);
        var result = guiMethod(value, options);

        EndHorizontal();

        return result;
    }

    /// <summary>
    /// Check for a drag and drop event and get the items if there were any.
    /// </summary>
    /// <returns>Objects that were dropped or null.</returns>
    public static Object[] CheckForDragDrop()
    {
        //Check for drag and drop.
        var eventType = Event.current.type;
        Object[] droppedObject = null;

        if(eventType == EventType.DragPerform)
        {
            if(DragAndDrop.objectReferences.Length != 0)
            {
                droppedObject = DragAndDrop.objectReferences;
            }
        }

        return droppedObject;
    }

    /// <summary>
    /// Check if mouse is at location that could be target of drag drop.
    /// </summary>
    /// <returns>True if location is a drag drop location.</returns>
    public static bool IsDragDropLocation()
    {
        var dropLocation = GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition);
        if(dropLocation)
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
        }

        return dropLocation;
    }

    /// <summary>
    /// Consume the DragPerform event so no other objects can use it.
    /// </summary>
    /// <param name="eventType">GUI Event Type.</param>
    /// <param name="acceptDrop">Flag indicating if drag was accepted.</param>
    public static void ConsumeDragDrop(EventType eventType, bool acceptDrop)
    {
        if(eventType == EventType.DragPerform && acceptDrop)
        {
            DragAndDrop.AcceptDrag();
            Event.current.Use();
        }
    }

    /// <summary>
    /// Create a level GUI element.
    /// </summary>
    /// <param name="text">Label text for this label.</param>
    /// <param name="toolTip">Tool tip text for mouse over information.</param>
    public static void FieldLabel(string text, string toolTip)
    {
        // If the text is null then there is nothing to display.
        if(text == null)
        {
            return;
        }

        // Create a GUI content object for the label.
        var labelContent = string.IsNullOrEmpty(toolTip) ? new GUIContent(text) : new GUIContent(text, toolTip);

        // Find the min and max width of the label content.
        float minWidth;
        float maxWidth;
        GUI.skin.label.CalcMinMaxWidth(labelContent, out minWidth, out maxWidth);

        // Create the label field with the content and set the width to the max width of the text.
        EditorGUILayout.LabelField(labelContent, GUILayout.MaxWidth(maxWidth));
    }
}