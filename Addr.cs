﻿using System;
using System.Threading;
namespace SanguoDotNet;


public class Addr 
{   
    private  Mutex mtx;
    private  LogicAddr _logicAddr;
    private  string _netAddr;
    public LogicAddr LogicAddr
    {
        get => _logicAddr;
    }

    public string NetAddr
    {
        get
        {
           string netAddr;
           mtx.WaitOne();
           netAddr = _netAddr;
           mtx.ReleaseMutex();
           return netAddr;
        }
        set
        {
            if(value != null)
            {
                mtx.WaitOne();
                _netAddr = value;
                mtx.ReleaseMutex();
            }            
        }
    }

    public Addr(string logicAddr,string netAddr)
    {   
        mtx = new Mutex();
        _logicAddr= new(logicAddr);
        _netAddr = netAddr;
    }

}

public class LogicAddr 
{
    public const uint ClusterMask = 0xFFFC0000; //高14
    public const uint TypeMask = 0x0003FC00;    //中8
    public const uint ServerMask = 0x000003FF;  //低10
    public const uint HarbarType = 255;
    private uint _addr; 
    public uint ToUint32() 
    {
        return _addr;
    }

    public uint Cluster() {
        return (_addr & ClusterMask) >> 18;
    }

    public uint Type(){
	    return (_addr & TypeMask) >> 10;
    }

    public uint Server(){
	    return _addr & ServerMask;
    }

    override public string ToString(){
        return $"{Cluster()}.{Type()}.{Server()}";
    }

    public LogicAddr(uint addr)
    {
        _addr = addr;
        if(Cluster() == 0 || Cluster() > (ClusterMask>>18))
        {
		    throw new ArgumentException("invaild logic addr:invaild cluster");
	    }

        if(Type() == 0 || Type() > ((TypeMask>>10))) 
        {
		    throw new ArgumentException("invaild logic addr:invaild type");
	    }

        if(Server() > ServerMask)
        {
            throw new ArgumentException("invaild logic addr:invaild server");
        }
    }

    public LogicAddr(string str) 
    {
        char[] charSeparators = new char[] { '.' };
        string[] result = str.Split(charSeparators);
        if(result.Length != 3) 
        {
            throw new ArgumentException("invaild logic addr format('n.n.n')");
        }

        uint cluster = (uint)Convert.ToInt32(result[0]);
        uint type = (uint)Convert.ToInt32(result[1]);     
        uint server = (uint)Convert.ToInt32(result[2]);

        if(cluster == 0 || cluster > (ClusterMask>>18))
        {
		    throw new ArgumentException("invaild logic addr:invaild cluster");
	    }

        if(type == 0 || type > ((TypeMask>>10))) 
        {
		    throw new ArgumentException("invaild logic addr:invaild type");
	    }

        if(server > ServerMask)
        {
            throw new ArgumentException("invaild logic addr:invaild server");
        }


        _addr = 0 | (type << 10) | (cluster << 18) | server;
    }
}

