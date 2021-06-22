using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour
{
	public float moveTime = 0.1f; //Lo que va a tardar nuestro objeto en moverse en segundos
	public LayerMask blockingLayer;//Layer on which collision will be checked.
	
	private BoxCollider2D boxCollider; //The BoxCollider2D component attached to this object.
	private Rigidbody2D rb2D;//The Rigidbody2D component attached to this object.
	private float inverseMoveTime;//Used to make movement more efficient.
	
	// Las funciones virtuales protegidas se pueden anular heredando clases.
	protected virtual void Start (){
		
		//Get a component reference to this object's BoxCollider2D
		boxCollider = GetComponent<BoxCollider2D>();
		
		//Get a component reference to this object's Rigidbody2D
		rb2D = GetComponent<Rigidbody2D>();

		//By storing the reciprocal of the move time we can use it by multiplying instead of dividing, this is more efficient.		
		inverseMoveTime = 1f/ moveTime;
	}
	
	//Move returns true if it is able to move and false if not. 
    //Move takes parameters for x direction, y direction and a RaycastHit2D to check collision.
	protected bool Move (int xDir, int yDir, out RaycastHit2D hit){
		
		//Store start position to move from, based on objects current transform position.
		Vector2 start = transform.position;
		
		// Calculate end position based on the direction parameters passed in when calling Move.
		Vector2 end = start + new Vector2 (xDir, yDir);
		
		// Deshabilita boxCollider para que linecast no golpee el propio colisionador de este objeto.
		boxCollider.enabled = false;
		
		// Lanza una línea desde el punto de inicio hasta el punto final comprobando la colisión en la capa de bloqueo.
		hit = Physics2D.Linecast (start, end, blockingLayer);//Que es linecast?
		
		 //Re-enable boxCollider after linecast
		boxCollider.enabled = true;
		
		 //Check if anything was hit
		if (hit.transform == null){
			
			//If nothing was hit, start SmoothMovement co-routine passing in the Vector2 end as destination
			StartCoroutine(SmoothMovement (end));
			
			//Return true to say that Move was successful
			return true;
			
		}
		//If something was hit, return false, Move was unsuccesful.
		return false;
	
	}
	//Co-routine for moving units from one space to next, takes a parameter end to specify where to move to.
	protected IEnumerator SmoothMovement (Vector3 end){
		// Calcula la distancia restante para moverse según la magnitud cuadrada de la diferencia entre la posición actual y el parámetro final.
        // Se usa la magnitud cuadrada en lugar de la magnitud porque es computacionalmente más económica.
		 float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
		 
		//While that distance is greater than a very small amount (Epsilon, almost zero):
		 while (sqrRemainingDistance > float.Epsilon){
			 
			 //Find a new position proportionally closer to the end, based on the moveTime
			 Vector3 newPosition = Vector3.MoveTowards (rb2D.position, end, inverseMoveTime*Time.deltaTime);
			 
			 //Call MovePosition on attached Rigidbody2D and move it to the calculated position.
			 rb2D.MovePosition(newPosition);
			 
			 //Recalculate the remaining distance after moving.
			 sqrRemainingDistance = (transform.position - end ).sqrMagnitude;
			 
			  //Return and loop until sqrRemainingDistance is close enough to zero to end the function
			 yield return null;
		 }
	}
	//The virtual keyword means AttemptMove can be overridden by inheriting classes using the override keyword.
    //AttemptMove takes a generic parameter T to specify the type of component we expect our unit to interact with if blocked (Player for Enemies, Wall for Player).
	protected virtual void AttemptMove <T>(int xDir, int yDir)
		where T :Component
	{
		// Hit almacenará todos los hits de nuestro linecast cuando se llame a Move.
		RaycastHit2D hit;
		
		//Set canMove to true if Move was successful, false if failed.
		bool canMove = Move (xDir,yDir,out hit);
		
		 //Check if nothing was hit by linecast
		if (hit.transform == null)
		
			//If nothing was hit, return and don't execute further code.
			return;
		
		//Get a component reference to the component of type T attached to the object that was hi
		T hitComponent = hit.transform.GetComponent<T>();
		
		 //If canMove is false and hitComponent is not equal to null, meaning MovingObject is blocked and has hit something it can interact with.
		if(!canMove && hitComponent != null)
			
			//Call the OnCantMove function and pass it hitComponent as a parameter.
			OnCantMove(hitComponent);
	}
	//The abstract modifier indicates that the thing being modified has a missing or incomplete implementation.
    //OnCantMove will be overriden by functions in the inheriting classes.
	protected abstract void OnCantMove <T> (T component)
		 where T : Component;
	
    // Start is called before the first frame update
    /*void Start()
    {
        
    }*/

    // Update is called once per frame
    /*void Update()
    {
        
    }*/
}
