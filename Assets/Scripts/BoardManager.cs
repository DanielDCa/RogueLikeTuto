using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
/*Hola esto es un comentario*/
public class BoardManager : MonoBehaviour
{
	[Serializable]
	public class Count{
		
		public int minimum;
		public int maximum;
		
		public Count(int min,int max){
			minimum=min;
			maximum=max;
			
		}
	}               		
	
	public int columns = 8;
	public int rows = 8;
	public Count wallCount= new Count(5,9);//Minimo de 5 paredes y maximo de 9
	public Count foodCount = new Count (1,5);
	public GameObject exit;//to hold the prefabs that we are going to spawn, to make up our game board- hasta outerWalltiles
	public GameObject[] floorTiles;
	public GameObject[] wallTiles;
	public GameObject[] foodTiles;
	public GameObject[] enemyTiles;
	public GameObject[] outerWallTiles;
	
	private Transform boardHolder;///A variable to store a reference to the transform of our Board object.
	private List <Vector3> gridPositions= new List<Vector3>();//Usaremos esto para rastrear las posibles diferentes posiciones  y hacer un seguimientos de si un objeto ha sido generado en esa posiciones
	
	void InitialiseList(){
		
		//Limpiar nuestra lista de posiciones de cuadricula
		gridPositions.Clear();
		
		//Bucle por el eje X (Columnas)
		for(int x=1; x < columns - 1; x++ ){
		
			for(int y = 1; y < rows - 1; y++ ){//Bucle por el eje Y (Filas)
				
				//Crea una lista de posbiles posiciones para ubicar paredes, enemigos etc.
				//En cada índice agregue un nuevo Vector3 a nuestra lista con las coordenadas xey de esa posición.
				gridPositions.Add(new Vector3(x,y,0f));
			
			}
			
		}
	}
	
	void BoardSetup(){//Configurar las paredes externas y el piso de nuestro marco del juego
		
		//Instantiate Board and set boardHolder to its transform.
		boardHolder = new GameObject("Board").transform;
		
		//Loop along x axis, starting from -1 (to fill corner) with floor or outerwall edge tiles.
		for(int x = -1; x < columns + 1; x++ ){
			
			//Loop along y axis, starting from -1 to place floor or outerwall tiles
			for(int y= -1; y < rows + 1; y++ ){
				
				//Choose a random tile from our array of floor tile prefabs and prepare to instantiate it.
				GameObject toInstantiate = floorTiles[Random.Range(0,floorTiles.Length)];//Indexar nuestro array a floortiles object, que nosotros elegiremos aleatoriamente (entre 0 y la longitud de floorTiles)
				
				//Check if we current position is at board edge, if so choose a random outer wall prefab from our array of outer wall tiles.
				if(x == -1|| x == columns || y == -1 || y == rows)
					toInstantiate = outerWallTiles[Random.Range(0,outerWallTiles.Length)];
				
				//Instantiate the GameObject instance using the prefab chosen for toInstantiate at the Vector3 corresponding to current grid position in loop, cast it to GameObject.
				GameObject instance = Instantiate(toInstantiate, new Vector3(x,y,0f), Quaternion.identity) as GameObject;
				//Intantiate lo que hace es clonar el objeto toInstantiate y retorna el CLON con un nuevo vector con las coordenadas y sin rotacion=Quaternion.identity
				
				//Set the parent of our newly instantiated object instance to boardHolder, this is just organizational to avoid cluttering hierarchy.
				instance.transform.SetParent(boardHolder);//No entiendo muy bien
			}
			
		}
	}
	Vector3 RandomPosition(){
		//Inicializa un numeto entero eligiendo de manera aleatoria un valor entre 0 y la cantidad de posiciones
		int randomIndex = Random.Range(0, gridPositions.Count);
		
		//Se declara una vaiable del tipo Vector3 y se toma un valor aleatoriao de nuestra lista gridPositions
		Vector3 randomPosition = gridPositions[randomIndex];
		
		//Removemos esa posicion de nuestra lista para que no se pueda reutilizar
		gridPositions.RemoveAt(randomIndex);
		
		return randomPosition;
		
	}
	//Acepta una variedad de objetos de juego para elegir junto con un rango mínimo y máximo para la cantidad de objetos a crear.
	void LayoutObjectAtRandom(GameObject[] tileArray, int minimum,int maximum){
		
		//Elije un número aleatorio de objetos para instanciar dentro de los límites mínimo y máximo
		int objectCount = Random.Range(minimum, maximum + 1);
		
		//Crear instancias de objetos hasta que se alcance el objeto límite elegido aleatoriamente
		for(int i = 0; i < objectCount; i++ ){
			
			//Se devulve una posicion aleatoria de nuestra funcion "RandomPosition"
			Vector3	randomPosition = RandomPosition();
			
			//Elije un mosaico aleatorio de tileArray y lo asigna  a tileChoice
			GameObject tileChoice = tileArray[Random.Range(0,tileArray.Length)];
			
			//Crea una instancia de tileChoice en la posición devuelta por RandomPosition sin cambios en la rotación
			Instantiate (tileChoice, randomPosition, Quaternion.identity); 
			
		}
		
	}
	public void SetupScene(int level){
		//Crea las paredes externas y el piso
		BoardSetup();
		
		//Resetea nuestra lista de gridPositions
		InitialiseList();
		
		//Crea una instancia de un número aleatorio de baldosas de pared según el mínimo y el máximo, en posiciones aleatorias.
		LayoutObjectAtRandom(wallTiles,wallCount.minimum, wallCount.maximum);
		
		//Crea una instancia de un número aleatorio de baldosas de comida según el mínimo y el máximo, en posiciones aleatorias.
		LayoutObjectAtRandom(foodTiles,foodCount.minimum, foodCount.maximum);
		
		//Determinar la cantidad de enemigos según el número de nivel actual, según una progresión logarítmica
		int enemyCount = (int)Mathf.Log(level, 2f);
		
		//Crea una instancia de un número aleatorio de enemigos según el mínimo y el máximo, en posiciones aleatorias
		LayoutObjectAtRandom(enemyTiles,enemyCount, enemyCount);
		
		//Crea una instancia de la ficha de salida en la esquina superior derecha de nuestro tablero de juego
		Instantiate(exit, new Vector3(columns - 1,rows - 1,0f), Quaternion.identity);
	}
	
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
