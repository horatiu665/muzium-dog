 --- NOTE ----

[SETUP]
 - Add the layer "Item" to your project's layers
 - On the Player prefab in your scene, assign the "itemLayer" of the PlayerPickup script to the layer "Items" you just created

[CREATE CUSTOM INTERACTABLE OBJECTS]

 - If you want to create your own interaction methods, create a new script inherited from "ItemBehaviour", and paste this into it :

    public override void Awake()
    {
        base.Awake();
    }

    public override void OnInteract()
    {
        // Stuff to do when you interact with object
    }

    public override void OnFocus()
    {
        // Stuff to do when you hover the object with mouse
    }

    public override void OnLoseFocus()
    {
        // Stuff to do when you're no longer looking at the object
    }

 - On the script you just created, you can choose with the boolean "pickableObject" if you want to make your custom object pickable or not
 ---> There is a demo object with a demo script on it, called DemoInteractableObject
	