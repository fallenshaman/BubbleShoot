using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Bubble : MonoBehaviour
{
    public enum ColorType
    {
        ORANGE = 0,
        RED,
        GREEN,
        BLUE,
        PURPLE,
        MAX_COUNT,
    }

    [SerializeField]
    private Rigidbody2D rigidbody;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    private ColorType colorType;

    public ColorType Color{
        get {
            return colorType;
        }
    }

    public Cell attachedCell = null;

    private static string ProjectileTag = "Projectile";
    private static string BubbleTag = "Bubble";

    public void SetProjectilBubble()
    {
        rigidbody.bodyType = RigidbodyType2D.Dynamic;
        gameObject.tag = ProjectileTag;
        gameObject.layer = LayerMask.NameToLayer(ProjectileTag);
    }

    public void SetBubble()
    {
        rigidbody.bodyType = RigidbodyType2D.Static;

        gameObject.tag = BubbleTag;
        gameObject.layer = LayerMask.NameToLayer(BubbleTag);
    }

    public void AddForce(Vector3 force)
    {
        rigidbody.AddForce(force);
    }
    
    public void ChangeColor()
    {
        int colorIndex = Random.Range(0, (int)ColorType.MAX_COUNT);
        ChangeColor(colorIndex);
    }

    public void ChangeColor(int colorIndex)
    {
        if (colorIndex < 0 || (int)ColorType.MAX_COUNT <= colorIndex)
            colorIndex = 0;

        spriteRenderer.sprite = GameManager.Instance.listBubbleSprite[colorIndex];
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            Vector2 vel = rigidbody.velocity;
            vel.x *= -1f;

            rigidbody.velocity = vel;
        }
        else if(collision.gameObject.CompareTag("DestroyArea"))
        {
            rigidbody.velocity = Vector2.zero;
            rigidbody.angularVelocity = 0f;

            SetBubble();
            PoolManager.Instance.GetPool("Bubble").Desapwn(this.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(gameObject.CompareTag("Projectile") && collision.gameObject.CompareTag("Bubble"))
        {
            transform.parent = GameManager.Instance.gameBoard.transform;
            
            Bubble hittedBubble = collision.gameObject.GetComponent<Bubble>();

            List<Cell> listAdjacentCells = GameManager.Instance.gameBoard.GetAdjacentCells(hittedBubble.attachedCell);


            float minDistance = float.MaxValue;
            Cell nearCell = null;

            foreach(Cell cell in listAdjacentCells)
            {
                if (cell == null)
                    continue;

                if (!cell.IsEmpty())
                    continue;

                Vector3 cellPos = cell.GetPositionOnGrid();

                float distance = Vector3.Distance(transform.localPosition, cellPos);

                Debug.Log(string.Format("[{0},{1}] {2}", cell.row, cell.col, distance));

                if(distance < minDistance)
                {
                    minDistance = distance;
                    nearCell = cell;
                }
            }

            SetBubble();
            nearCell.AttachBubble(this);
            
        }
    }


}
