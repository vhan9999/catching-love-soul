using System.Collections;
using UnityEngine;
namespace GameCore.Boss
{

    [RequireComponent(typeof(BossController))]
    public class BossAIController : MonoBehaviour {
        
        IBossController bossController;
        [SerializeField] int currentState = 0;
        [SerializeField] Transform player;
        private void Start() {
            bossController = GetComponent<BossController>();
            
        }
        private void Update() {
            
        }
        
        public void MakeDecision(){
            if(currentState>=1){
                if(closeToPlayer()){
                    LookatPlayer();
                    bossController.OnAttack(0,()=>{
                        MoveToPlayer(()=>{
                            if(currentState==3){
                                bossController.OnAttack(2,()=>{
                                    //back to idle
                                    RestartDecision();
                                });
                            }
                            else{
                                //back to idle
                                RestartDecision();
                            }
                        });
                    });
                }
                else{
                    MoveRandomly(()=>{
                        if(currentState >= 2){
                            bossController.OnAttack(1,()=>{
                                MoveAwayFromPlayer(()=>{
                                    RushToPlayer(()=>{
                                        //back to idle
                                        RestartDecision();
                                    });
                                });
                            });
                        }
                        else{
                            RushToPlayer(()=>{
                                //back to idle
                                RestartDecision();
                            });
                        }   
                    });
                }
            }
        }
        void RestartDecision(){
            bossController.OnIdle();
            StartCoroutine(waitForNextLoop());
            IEnumerator waitForNextLoop(){
                yield return new WaitForSeconds(3);
                Debug.Log("restart");
                MakeDecision();
            }
        }
        bool closeToPlayer(){
            Vector2 playerPos = player.position;
            Vector2 bossPos = transform.position;
            float minDistance =3f;
            return Vector2.Distance(playerPos,bossPos) > minDistance;
        }
        void MoveAwayFromPlayer(System.Action onComplete){
            Vector2 playerPos = player.position;
            Vector2 bossPos = transform.position;
            float moveSecScale = .5f;
            bossController.OnMove( 
                (- new Vector2(playerPos.x-bossPos.x,playerPos.y-bossPos.y)).normalized,
                onComplete,
                Vector2.Distance(playerPos,bossPos)*moveSecScale);
        }
        void LookatPlayer(){
            Vector2 playerPos = player.position;
            Vector2 bossPos = transform.position;
            Vector2 direction =new Vector2(playerPos.x-bossPos.x,playerPos.y-bossPos.y).normalized;
            bossController.TurnAround(direction.x>0);
        }
        void MoveToPlayer(System.Action onComplete){
            Vector2 playerPos = player.position;
            Vector2 bossPos = transform.position;
            float moveSecScale = .5f;
            float maximaMoveDuration = 3;
            bossController.OnMove( 
                new Vector2(playerPos.x-bossPos.x,playerPos.y-bossPos.y).normalized,
                onComplete,
                Mathf.Min(maximaMoveDuration,Vector2.Distance(playerPos,bossPos)*moveSecScale));
        }
        void MoveRandomly(System.Action onComplete){
            bossController.OnMove(
                new Vector2(Random.Range(0,10),Random.Range(0,10)).normalized,
                onComplete,
                Random.Range(1,3));
        }   
        void RushToPlayer(System.Action onComplete){
            Vector2 playerPos = player.position;
            Vector2 bossPos = transform.position;
            float rushScale = 2f;
            float rushSec = 2;
            bossController.OnRush( 
                new Vector2(playerPos.x-bossPos.x,playerPos.y-bossPos.y).normalized * rushScale,
                onComplete,
                rushSec);
        }
    }
}