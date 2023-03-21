using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO; //aggiunto namespace mancante per FileStream

public static class SaveSystem
{
public static void SavePlayer(Move player)
{
BinaryFormatter formatter = new BinaryFormatter();
string path = Application.persistentDataPath + "/player.fun"; //corretto "persistendDatapath" con "persistentDataPath"
FileStream stream = new FileStream(path, FileMode.Create);
PlayerData data = new PlayerData(player);
formatter.Serialize(stream, data);
stream.Close();
}
public static PlayerData LoadPlayer()
{
    string path = Application.persistentDataPath + "/player.fun"; //corretto "persistendDatapath" con "persistentDataPath"
    if(File.Exists(path))
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Open);
        PlayerData data = formatter.Deserialize(stream) as PlayerData; //corretto "Deserialiize" con "Deserialize"
        stream.Close();
        return data;
    } 
    else
    {
        Debug.LogError("Save file not found in " + path); //aggiunto spazio dopo "in"
        return null;
    }
}



}
