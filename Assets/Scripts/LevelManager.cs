using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField] private FieldGenerator _fieldGenerator;
    [SerializeField, Min(1)]  private int _startLevel = 1;
    [SerializeField] private int sizeBonus = 0;

    public int CurrentLevel { get; private set; }
    
    private void Awake()
    {
        LoadLevel(_startLevel);
    }
    
    public void NextLevel()
    {
        LoadLevel(CurrentLevel + 1);
    }
    
    public void LoadLevel( int level )
    {
        CurrentLevel = level;
        _fieldGenerator.Generate(CurrentLevel, sizeBonus);    
    }
}
