using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockPool : ObjectPool<BlockControl>
{
    private int block_count = 0; // 생성한 블록의 개수.
    new protected int additionCount = 2;

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
