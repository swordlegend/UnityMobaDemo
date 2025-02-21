﻿/****************************************************
    文件：GMSystem.cs
    作者：CodingCodingK
    博客：CodingCodingK.github.io
    邮箱：2470939431@qq.com
    日期：#DATE#
    功能：模拟服务器 测试用Sys
*****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using proto.HOKProtocol;
using UnityEngine;

public class GMSystem : SystemBase
{
    public static GMSystem Instance;
    public bool isActive = false;
    private int frameId = 0;
    private List<OpKey> opkeyList = new List<OpKey>();

    public override void InitSys()
    {
        base.InitSys();
        
        Instance = this;
        this.Log("GMSystem Init Completed.");
    }

    public void StartSimulate()
    {
        isActive = true;
        StartCoroutine(BattleSimulate());
    }

    public IEnumerator BattleSimulate()
    {
        SimulateLoadRes();
        yield return new WaitForSeconds(2f);
        SimulateBattleStart();
    }

    void SimulateLoadRes()
    {
        var msg = new GameMsg
        {
            cmd = CMD.NotifyLoadRes,
            notifyLoadRes = new NotifyLoadRes()
            {
                mapId = 101,
                heroList = new List<BattleHeroData>()
                {
                    new BattleHeroData{ heroId = 103,userName = "P1"},
                    new BattleHeroData{ heroId = 103,userName = "P2"},
                    new BattleHeroData{ heroId = 103,userName = "P3"},
                    new BattleHeroData{ heroId = 102,userName = "P4"},
                    new BattleHeroData{ heroId = 101,userName = "P5"},
                    new BattleHeroData{ heroId = 102,userName = "P6"},

                },
                posIndex = 0
            }
        };
        
        LobbySys.Instance.NotifyLoadRes(msg);
    }

    void SimulateBattleStart()
    {
        var msg = new GameMsg()
        {
            cmd = CMD.RspBattleStart
        };
        BattleSys.Instance.RspBattleStart(msg);
    }

    public void SimulateServerRcvMsg(GameMsg msg)
    {
        switch (msg.cmd)
        {
            case CMD.SendOpKey:
                UpDateOpKey(msg.sendOpKey.opKey);
                break;
        }
    }

    void UpDateOpKey(OpKey key)
    {
        opkeyList.Add(key);
    }

    private void FixedUpdate()
    {
        if (isActive == false)
        {
            return;
        }
        
        ++frameId;
        GameMsg msg = new GameMsg()
        {
            cmd = CMD.NotifyOpKey,
            notifyOpKey = new NotifyOpKey()
            {
                frameId = frameId,
                keyList = new List<OpKey>(),
            }
        };

        if (opkeyList.Count > 0)
        {
            msg.notifyOpKey.keyList.AddRange(opkeyList);
        }
        opkeyList.Clear();
        netSvc.AddNetMsg(msg);
        
    }

    private void Update()
    {
        
    }
}
