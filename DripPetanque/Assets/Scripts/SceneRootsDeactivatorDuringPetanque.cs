using System;

public class SceneRootsDeactivatorDuringPetanque : SceneRootsDeactivator
{
    void Start()
    {
        PetanqueSubGameManager petanqueSubGameManager = GameManager.Instance.PetanqueSubGameManager;
        petanqueSubGameManager.OnPetanqueSceneLoaded += OnPetanqueSceneLoaded;
        petanqueSubGameManager.ReactivateMainScene += ReactivateScene; 

    }

    private void ReactivateScene()
    {
        ResetRootsStates();
    }

    private void OnPetanqueSceneLoaded()
    {
        DisableSceneRoots();
    }
}
