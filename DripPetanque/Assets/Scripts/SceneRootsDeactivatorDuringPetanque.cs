public class SceneRootsDeactivatorDuringPetanque : SceneRootsDeactivator
{
    private void Start()
    {
        PetanqueSubGameManager petanqueSubGameManager = GameManager.Instance.PetanqueSubGameManager;
        petanqueSubGameManager.OnPetanqueSceneLoaded += OnPetanqueSceneLoaded;
        petanqueSubGameManager.ReactivateMainScene += ReactivateScene;
    }

    private void OnDestroy()
    {
        if (GameManager.ApplicationIsQuitting)
        {
            return;
        }
        PetanqueSubGameManager petanqueSubGameManager = GameManager.Instance.PetanqueSubGameManager;
        petanqueSubGameManager.OnPetanqueSceneLoaded -= OnPetanqueSceneLoaded;
        petanqueSubGameManager.ReactivateMainScene -= ReactivateScene;
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
