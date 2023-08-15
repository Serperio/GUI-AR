//Objeto con los datos de una Red WIFI en la BD
public class WifiData{
    public string mac;
    public int intensity;
    public int floor;

    public WifiData(string _mac, int _intensity, int _floor){
        mac = _mac;
        intensity = _intensity;
        floor = _floor;
    }
}