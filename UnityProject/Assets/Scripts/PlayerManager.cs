using System.Collections;
using System.Collections.Generic;
using MyBox;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    List<Player> _players = new List<Player>();
    public List<Player> Players {
        get => _players;
        set => _players = value;
    }

    int _currentPlayer;

    public void SelectNextPlayer() {
        foreach (Player player in _players) {
            player.SelectPlayer(false);
        }
        ++_currentPlayer;
        _currentPlayer %= _players.Count;

        _players[_currentPlayer].SelectPlayer(true);
    }
}

