using UnityEngine;

public class DogEmotionBrain : MonoBehaviour
{

    [SerializeField]
    private DogRefs _dogRefs;
    public DogRefs dogRefs
    {
        get
        {
            if (_dogRefs == null)
            {
                _dogRefs = GetComponent<DogRefs>();
            }
            return _dogRefs;
        }
    }

    

    public void AddHappiness(Component who, float amount)
    {
// TODO gotta add happiness and wag tail...?!?!
    }

}