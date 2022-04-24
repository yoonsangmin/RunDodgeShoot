using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockPool : ObjectPool<BlockControl>
{
    private int block_count = 0; // 생성한 블록의 개수.
    // new 키워드를 쓰면 부모의 변수와 함수를 가려준다. 없어도 작동은 똑같지만 컴파일러와 다른 사람들에게 의도적임을 알려준다.
    protected new int additionCount = 2;

    [SerializeField]
    private BlockControl[] blocks;

    protected override void Allocate()
    {
        // 만들어야 할 블록의 종류(흰색인가 빨간색인가)를 구한다.
        int next_block_type = this.block_count % blocks.Length;

        poolObject = blocks[next_block_type];

        base.Allocate();

        this.block_count++; // 블록의 개수를 증가.
    }
}
