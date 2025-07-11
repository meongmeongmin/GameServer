using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class GameRoom : IJobQueue
{
    List<ClientSession> _sessions = new List<ClientSession>();
    JobQueue _jobQueue = new JobQueue();
    List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();

    public void Push(Action job)
    {
        _jobQueue.Push(job);
    }

    public void Flush()
    {
        foreach (ClientSession s in _sessions)
            s.Send(_pendingList);
        //Console.WriteLine($"Flushed {_pendingList.Count} items");
        _pendingList.Clear();
    }

    public void Broadcast(ArraySegment<byte> segment)
    {
        _pendingList.Add(segment);
    }

    public void Enter(ClientSession session)
    {
        // 플레이어 추가하고
        _sessions.Add(session);
        session.Room = this;

        // 신입생한테 모든 플레이어 목록 전송
        S_PlayerList players = new S_PlayerList();
        foreach (ClientSession s in _sessions)
        {
            players.Players.Add(new S_PlayerList.Player()
            {
                IsSelf = (s == session),
                PlayerId = s.SessionId,
                PosX = s.PosX,
                PosY = s.PosY,
                PosZ = s.PosZ,
            });
        }

        session.Send(players.Write());

        // 신입생 입장을 모두에게 알린다
        S_BroadcastEnterGame enter = new S_BroadcastEnterGame();
        enter.PlayerId = session.SessionId;
        enter.PosX = 0;
        enter.PosY = 0;
        enter.PosZ = 0;
        Broadcast(enter.Write());
    }

    public void Leave(ClientSession session)
    {
        // 플레이어 제거하고
        _sessions.Remove(session);

        // 모두에게 알린다
        S_BroadcastLeaveGame leave = new S_BroadcastLeaveGame();
        leave.PlayerId = session.SessionId;
        Broadcast(leave.Write());
    }

    public void Move(ClientSession session, C_Move packet)
    {
        // 좌표 바꿔주고
        session.PosX = packet.PosX;
        session.PosY = packet.PosY;
        session.PosZ = packet.PosZ;

        // 모두에게 알린다
        S_BroadcastMove move = new S_BroadcastMove();
        move.PlayerId = session.SessionId;
        move.PosX = packet.PosX;
        move.PosY = packet.PosY;
        move.PosZ = packet.PosZ;
        Broadcast(move.Write());
    }
}