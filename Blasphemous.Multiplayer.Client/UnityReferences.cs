using Framework.Managers;
using Gameplay.UI.Others.UIGameLogic;
using UnityEngine;

namespace Blasphemous.Multiplayer.Client;

/// <summary>
/// Stores references for necessary unity objects
/// </summary>
public static class UnityReferences
{
    /// <summary>
    /// Used to place nametags
    /// </summary>
    public static Transform CanvasObject
    {
        get
        {
            if (m_canvas == null)
            {
                foreach (Canvas c in Object.FindObjectsOfType<Canvas>())
                {
                    if (c.name == "Game UI")
                    {
                        m_canvas = c.transform;
                        break;
                    }
                }
            }

            return m_canvas;
        }
    }
    private static Transform m_canvas;

    /// <summary>
    /// Used to place nametags
    /// </summary>
    public static GameObject TextObject
    {
        get
        {
            if (m_textObject == null)
            {
                foreach (PlayerPurgePoints obj in Object.FindObjectsOfType<PlayerPurgePoints>())
                {
                    if (obj.name == "PurgePoints")
                    {
                        m_textObject = obj.transform.GetChild(1).gameObject;
                        break;
                    }
                }
            }

            return m_textObject;
        }
    }
    private static GameObject m_textObject;

    /// <summary>
    /// Used to show other players
    /// </summary>
    public static RuntimeAnimatorController PlayerAnimator
    {
        get
        {
            if (m_playerAnimator == null)
            {
                m_playerAnimator = Core.Logic.Penitent?.Animator.runtimeAnimatorController;
            }
            return m_playerAnimator;
        }
    }
    private static RuntimeAnimatorController m_playerAnimator;

    /// <summary>
    /// Used to show other players
    /// </summary>
    public static RuntimeAnimatorController PlayerSwordAnimator
    {
        get
        {
            if (m_SwordAnimator == null)
            {
                m_SwordAnimator = Core.Logic.Penitent?.GetComponentInChildren<Gameplay.GameControllers.Penitent.Attack.SwordAnimatorInyector>().GetComponent<Animator>().runtimeAnimatorController;
            }
            return m_SwordAnimator;
        }
    }
    private static RuntimeAnimatorController m_SwordAnimator;

    /// <summary>
    /// Used to show other players
    /// </summary>
    public static Material PlayerMaterial
    {
        get
        {
            if (m_playerMaterial == null)
            {
                m_playerMaterial = Core.Logic.Penitent?.SpriteRenderer.material;
            }
            return m_playerMaterial;
        }
    }
    private static Material m_playerMaterial;
}
