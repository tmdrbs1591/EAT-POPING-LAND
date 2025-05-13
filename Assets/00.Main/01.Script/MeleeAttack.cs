using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
  public PlayerBattle playerBattle;

    public void Attack()
    {
        playerBattle.Attack();
    }
}
