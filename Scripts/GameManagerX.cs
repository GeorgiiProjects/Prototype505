using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManagerX : MonoBehaviour
{
    // создаем класс TextMeshProUGUI для того чтобы поместить в инспекторе в него объект Score Text.
    public TextMeshProUGUI scoreText;
    // создаем класс TextMeshProUGUI для того чтобы поместить в инспекторе в него объект GameOver Text.
    public TextMeshProUGUI gameOverText;
    // создаем класс TextMeshProUGUI для того чтобы поместить в инспекторе в него объект CountDown Text.
    public TextMeshProUGUI countDownText;
    // создаем класс GameObject titleScreen, для того чтобы поместить в инспекторе в него объект Title Screen.
    public GameObject titleScreen;
    // создаем класс Button для того чтобы поместить в инспекторе в него кнопку Restart Button.
    public Button restartButton;
    // создаем класс лист для того чтобы поместить в инспекторе в него префабы еды и черепа.
    public List<GameObject> targetPrefabs;
    // создаем переменную, чтобы поместить в нее счет в игре.
    private int score;
    // создаем переменную для того чтобы объекты/префабы спавнились через 1,5 секунды.
    private float spawnRate = 1.5f;
    // создаем переменную для того чтобы определить окончена игра или нет.
    public bool isGameActive;
    // создаем переменную для того чтобы можно было начать отсчет времени в игре.
    private float countDown;
    // Минимальное значение появления объектов/префабов по оси х.
    private float minValueX = -3.75f;
    // Минимальное значение появления объектов/префабов по оси у.
    private float minValueY = -3.75f;
    // Расстояние появления между объектами/префабами.
    private float spaceBetweenSquares = 2.5f;

    // создаем метод StartGame, чтобы игра могла начаться, передаем параметр difficulty, чтобы выбрать уровень сложности при старте игры.
    // делаем метод публичным так как понадобится его вызывать в скрипте DifficultyButtonX для выбора уровня сложности.
    public void StartGame(int difficulty)
    {
        // игра активна при старте, создаем данное поле первым в методе так как корутина ни всегда понимает очередность запуска в методе.
        isGameActive = true;
        // быстроту спавна объектов 1 делим на уровень сложности 1,2,3 в зависимости от выбора уровня сложности, скорость спавна объектов будет меняться.
        spawnRate /= difficulty;
        // отсчет врмени при старте игры начинается с 60 секунд и идет до 0.
        countDown = 60;
        // счет при старте игры 0, создаем данное поле до корутины в методе так как корутина ни всегда понимает очередность запуска в методе.
        score = 0;
        // запускаем таймер спавна объектов 1 секунда и идет спавн.
        StartCoroutine(SpawnTarget());
        // счет при старте игры 0 будет обновляться.
        UpdateScore(0);
        // Пока игра работает титульный экран отключен.
        titleScreen.SetActive(false);
    }

    private void Update()
    {
        // если игра активна
        if (isGameActive)
        {
            // обновляем отсчет времени.
            UpdateCountDown();
        }       
    }

    // создаем метод для обновления отсчета времени в игре.
    public void UpdateCountDown()
    {
        // обновляем отсчет времени со скоростью 1 единица в секунду.
        countDown -= 1 * Time.deltaTime;
        // обновляем отсчет от 60 до 0 (59,58,57 итд.)
        countDownText.text = "Countdown: " + Mathf.Round(countDown);
        // если отсчет времени <= 0
        if (countDown <= 0)
        {
            // игра заканчивается
            GameOver();
        }          
    }

    // создаем курутину/интерфейс SpawnTarget(), для того чтобы контролировать скорость спавна объектов.
    IEnumerator SpawnTarget()
    {
        // пока эти условия соблюдаются в цикле while и игра не окончена
        while (isGameActive)
        {
            // скорость спавна каждые 1,5 секунды.
            yield return new WaitForSeconds(spawnRate);
            // выбираем рандомный объект из 4 для появления на экране, используем targets.Count так как они берутся из листа.
            int index = Random.Range(0, targetPrefabs.Count);
            // если игра активна
            if (isGameActive)
            {
                // создаем копии префабов/объектов через массив, используя его координаты и появление в рандомных координатах, поворот оставляем по умолчанию.
                Instantiate(targetPrefabs[index], RandomSpawnPosition(), targetPrefabs[index].transform.rotation);
            }
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
        // Генерируем рандомную позицию спавна объектов/префабов по осям x,y, по оси z значение остается неизменным.
        Vector3 spawnPosition = new Vector3(spawnPosX, spawnPosY, 0);
        // Используем в игре полученные значения.
        return spawnPosition;
    }

    // Метод для рандомной генерации позиции в 1 из 4 квадратов.
    int RandomSquareIndex()
    {
        // Генерируем рандомную позицию спавна объектов/префабов в 1 из 4 квадратов по осям х и у.
        return Random.Range(0, 4);
    }

    // создаем метод для обновления счета в игре при уничтожении префабов/объектов, используем параметр scoreToAdd  
    // для того чтобы счет прибавлялся или убавлялся, а не просто показывал начальный счет 0.
    // делаем метод публичным так как понадобится его вызывать в скрипте TargetX для обновления счета.
    public void UpdateScore(int scoreToAdd)
    {
        // счет во время игры обновляется на 5, 10, 15 при уничтожении префабов Good и Bad.
        score += scoreToAdd;
        // инициализируем текст, который будет появляться на экране в игре.
        scoreText.text = "Score: " + score;
    }

    // создаем метод GameOver() для отображения Game Over по окончанию игры.
    // делаем метод публичным так как понадобится его вызывать в скрипте TargetX.
    public void GameOver()
    {
        // Restart отображается по окончанию игры, так как кнопка содержащая текст Restart становится активной.
        restartButton.gameObject.SetActive(true);
        // Game Over отображается по окончанию игры, так как объект содержащий текст GameOver Text становится активным.
        gameOverText.gameObject.SetActive(true);
        // игра окончена.
        isGameActive = false;
    }

    // Создаем метод RestartGame() для перезапуска игры по нажатию кнопки Restart в игре.
    private void RestartGame()
    {
        // Перезагружаем текущий уровень, для этого используем класс SceneManager, который понимает какая сцена используется GetActiveScene(), 
        // исходя из ее имени .name
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
