using System.Collections;
using UnityEngine;

public class TargetX : MonoBehaviour
{
    // создаем класс Rigidbody, для последующего доступа к нему в префабах еды и черепа.
    private Rigidbody targetRb;
    // оставляем ссылку на класс GameManager для того чтобы его можно было использовать в этом скрипте.
    private GameManagerX gameManagerX;
    // создаем переменную чтобы определить сколько очков будет давать или отнимать префаб еды или черепа в игре (выставляем значения в префабах).
    public int pointValue;
    // создаем класс GameObject explosionFx, для того чтобы поместить в инспекторе в него префаб Explosion при уничтожении объекта.
    public GameObject explosionFx;
    // Объекты/префабы Cookie, Pizza, Skull и Steak находятся на экране 1 секунду.
    public float timeOnScreen = 1.0f;
    // Минимальное значение появления объектов/префабов по оси х.
    private float minValueX = -3.75f;
    // Минимальное значение появления объектов/префабов по оси у.
    private float minValueY = -3.75f;
    // Расстояние появления между объектами/префабами.
    private float spaceBetweenSquares = 2.5f;
    
    void Start()
    {
        // получаем доступ к классу Rigidbody, чтобы можно было использовать различные силы на объектах/префабах еды и черепа.
        targetRb = GetComponent<Rigidbody>();
        // получаем доступ к Game Manager в иерархии, через класс GameObject.Find и строку "Game Manager", получаем доступ к скрипту GameManagerX.
        gameManagerX = GameObject.Find("Game Manager").GetComponent<GameManagerX>();
        // объекты/префабы еды и черепа будут спавниться в координатах по x от -4 до 4, по оси y -2.
        transform.position = RandomSpawnPosition();
        // запускаем курутину, таймер исчезновения объектов/префабов 1 секунда и происходит исчезновение.
        StartCoroutine(RemoveObjectRoutine());
    }

    // метод нажатия левой кнопки мыши.
    private void OnMouseDown()
    {
        // если игра активна то (из класса/скрипта gameManagerX вызываем публичное поле isGameActive)
        if (gameManagerX.isGameActive)
        {
            // уничтожается объект/префаб еды или черепа на экране.
            Destroy(gameObject);
            // после уничтожения объекта/префабов дается или отнимается определенное кол-во очков.
            gameManagerX.UpdateScore(pointValue);
            // происходит эффект взрыва 
            Explode();
        }        
    }

    // создаем метод RandomSpawnPosition(), для того чтобы объекты/префабы появлялись в рандомных координатах по осям x и у от -3.75 до 3.75, по оси z 0.
    // расстояние появления между квадратами 2.5
    Vector3 RandomSpawnPosition()
    {
        // Вычисляем позицию спавна по оси х, для этого используем формулу.
        float spawnPosX = minValueX + (RandomSquareIndex() * spaceBetweenSquares);
        // Вычисляем позицию спавна по оси у, для этого используем формулу.
        float spawnPosY = minValueY + (RandomSquareIndex() * spaceBetweenSquares);
        // Генерируем рандомную позицию спавна объектов/префабов по осям x,y, по оси z значение остается неизвенным.
        Vector3 spawnPosition = new Vector3(spawnPosX, spawnPosY, 0);
        // Используем в игре полученные значения.
        return spawnPosition;
    }

    // Метод для рандомной генерации позиции в 1 из 4 квадратов.
    int RandomSquareIndex ()
    {
        // Генерируем рандомную позицию спавна объектов/префабов в 1 из 4 квадратов по осям х и у.
        return Random.Range(0, 4);
    }

    // когда коллайдер игровых объектов соприкасается с коллайдером сенсор
    private void OnTriggerEnter(Collider other)
    {
        // уничтожаются все оставшиеся игровые объекты.
        Destroy(gameObject);
        // если объект содержит тэг Sensor и если префаб не содержит тэг Bad
        if (other.gameObject.CompareTag("Sensor") && !gameObject.CompareTag("Bad"))
        {
            // отображается надпись GameOver при соприкосновении коллайдера префабов еды с коллайдером объекта сенсор.
            gameManagerX.GameOver();
        } 
    }

    // Метод отображения эффекта взрыва в позиции префаба еды или черепа
    void Explode ()
    {
        // создаем копии эффекта при уничтожении объекта, используя его координаты, поворот оставляемпо умолчанию.
        Instantiate(explosionFx, transform.position, explosionFx.transform.rotation);
    }

    // создаем курутину/интерфейс RemoveObjectRoutine (), для того чтобы контролировать скорость исчезновения объектов/префабов.
    IEnumerator RemoveObjectRoutine ()
    {
        // время исчезновения объектов/префабов через 1 секунду.
        yield return new WaitForSeconds(timeOnScreen);
        // Если игра активна (из класса/скрипта gameManagerX вызываем публичное поле isGameActive)
        if (gameManagerX.isGameActive)
        {
            // Перемещаем объект/префаб за видимую сцену, для взаимодействия с объектом сенсор.
            transform.Translate(Vector3.forward * 5, Space.World);
        }
    }
}
