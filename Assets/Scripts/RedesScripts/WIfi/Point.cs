//Objeto con los datos de un punto de GPS
public class Point{
    public float x;
    public float y;
    public string name;
    public int floor;
    public string tipo;

    public Point(float x_aux, float y_aux, string name_aux)
    {
        x = x_aux;
        y = y_aux;
        name = name_aux;
    }
}