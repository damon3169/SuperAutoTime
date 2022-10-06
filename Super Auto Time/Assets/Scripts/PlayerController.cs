using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public struct Unite
{
    public string name;
    public int health;
    public int damages;

    public Unite(string name, int health, int damages)
    {
        this.name = name;
        this.health = health;
        this.damages = damages;
    }
}
public class PlayerController : NetworkBehaviour
{

    public readonly SyncList<Unite> board = new SyncList<Unite>();
    public List<TimeUnite> boardList;

    // Start is called before the first frame update
    public override void OnStartClient()
    {
        board.Callback += OnBoardUpdated;
        for (int index = 0; index < board.Count; index++)
            OnBoardUpdated(SyncList<Unite>.Operation.OP_ADD, index, new Unite(), board[index]);
    }

    // Update is called once per frame
    void Update()
    {
        if (isLocalPlayer)
        {
            if (Input.GetMouseButtonDown(0))
            {
                foreach (TimeUnite unite in boardList)
                {
                    addNewUnite(unite.name, unite.health, unite.damages);
                }
            }
            if (Input.GetMouseButtonDown(1))
            {

                RemoveUnite(0);

            }
            if (board.Count > 0)
            {
                foreach (Unite unite in board)
                {
                    Debug.Log("name="+ unite.name + " health =" + unite.health.ToString()+"damages= "+unite.damages.ToString());
                }
            }
        }
    }
    void OnBoardUpdated(SyncList<Unite>.Operation op, int index, Unite oldUnite, Unite newUnite)
    {
        switch (op)
        {
            case SyncList<Unite>.Operation.OP_ADD:
                break;
            case SyncList<Unite>.Operation.OP_INSERT:
                break;
            case SyncList<Unite>.Operation.OP_REMOVEAT:
                break;
            case SyncList<Unite>.Operation.OP_SET:
                break;
            case SyncList<Unite>.Operation.OP_CLEAR:
                break;
        }
    }

    [Command]
    void addNewUnite(string newName, int newHealth, int newDamages)
    {
        board.Add(new Unite(newName, newHealth, newDamages));
    }

    [Command]
    void RemoveUnite(int unitePlacement)
    {
        board.RemoveAt(unitePlacement);
    }

}
