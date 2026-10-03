using System;
using System.IO;
using System.Text;
using Eclipse.Multiplayer.Online;

internal static class BalanceProtocolTests
{
    public static void Run(Action<bool,string> check)
    {
        string hash = new string('a',64);
        var lobby = new LobbyState { Arena="dojo", BalanceHash=hash, BalanceName="Tournament" };
        var bytes=lobby.Encode();
        var read=LobbyState.Decode(new NetReader(bytes,1,bytes.Length-1));
        check(read.BalanceHash==hash && read.BalanceName=="Tournament", "lobby balance identity round trip");
        // Names are limited by characters in JSON; UTF8 names can occupy nearly 200 bytes.
        lobby.BalanceName=new string('界',64);
        bytes=lobby.Encode();
        read=LobbyState.Decode(new NetReader(bytes,1,bytes.Length-1));
        check(read.BalanceName==lobby.BalanceName,"Unicode profile name fits lobby capacity");
        bytes=NetMessages.GuestLobby(LoadoutCode.None,true,hash);
        var reader=new NetReader(bytes,1,bytes.Length-1);
        LoadoutCode.Read(reader);
        check(reader.Bool() && reader.Str()==hash && reader.Remaining==0,"readiness acknowledges exact gameplay rules");
        bytes=new MatchStart {Arena="dojo", BalanceHash=hash}.Encode();
        var match=MatchStart.Decode(new NetReader(bytes,1,bytes.Length-1));
        check(match.BalanceHash==hash,"match start freezes balance hash");
        var replay=new VersusReplay {BalanceHash=hash,BalanceJson="{\"schemaVersion\":1,\"id\":\"example\",\"name\":\"Example\"}"};
        replay.Record(0,1);
        var stream=new MemoryStream(); replay.Write(stream); stream.Position=0;
        var loaded=VersusReplay.Read(stream);
        check(loaded.SourceFormat==3 && loaded.BalanceHash==hash && loaded.BalanceJson==replay.BalanceJson,"replay retains full profile snapshot");
        stream.Position=0; var header=VersusReplay.ReadHeader(stream);
        check(header.BalanceJson==replay.BalanceJson && header.BalanceHash==hash,"browser header retains balance metadata");
        // Format 2 has the same leading header, without its last two strings.
        byte[] all=stream.ToArray(); int oldLength=BitConverter.ToInt32(all,7);
        var suffix=new MemoryStream(); using(var writer=new BinaryWriter(suffix,Encoding.UTF8,true)) {writer.Write(hash);writer.Write(replay.BalanceJson);}
        int prefixLength=oldLength-(int)suffix.Length;
        var legacy=new MemoryStream(); legacy.Write(all,0,6); legacy.WriteByte(2);
        legacy.Write(BitConverter.GetBytes(prefixLength),0,4); legacy.Write(all,11,prefixLength);
        legacy.Write(all,11+oldLength,all.Length-11-oldLength); legacy.Position=0;
        var old=VersusReplay.Read(legacy);
        check(old.SourceFormat==2 && old.BalanceJson=="" && old.BalanceHash=="" && old.TickCount==1,"legacy format 2 can still be browsed");
    }
}
