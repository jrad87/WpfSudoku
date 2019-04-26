namespace SudokuAlgorithm

module RecursiveBacktrackingSolver =
    let g: int[] = Array.zeroCreate 81
   
    let checkRow (arr: int[]) row value =
        [for c in 0 .. 8 -> arr.[9 * row + c]] |>
        List.exists (fun x -> x = value)|> not
    
    let checkCol (arr: int[]) col value =    
        [for r in 0 .. 8 -> arr.[9 * r + col]] |>
        List.exists (fun x -> x = value) |> not

    let checkSector (arr: int[]) row col value =
        let rowStart, colStart = row - (row % 3), col - (col % 3)
        [for r in rowStart .. rowStart + 2 do 
            for c in colStart .. colStart + 2 -> arr.[9*r + c]] |>
        List.exists (fun x -> x = value) |> not  

    let isValidPlacement arr row col value = 
        let result = 
            checkRow arr row value &&
            checkCol arr col value &&
            checkSector arr row col value
        //printfn "%A" <| (row, col, value, result)
        result
    
    let rec backtrack (grid: int[]) cont index value =    
        if index = 81 then true
        elif grid.[index] <> 0 then backtrack grid cont (index + 1) 1
        else
            let row, col = index / 9, index % 9
            if isValidPlacement grid row col value 
            then
                grid.[index] <- value
                if backtrack grid cont (index + 1) 1 then true else grid.[index] <- 0; cont grid index value
            else
                cont grid index value

    let rec nextValue grid index value =
        if (value + 1) < 10 then backtrack grid nextValue index (value + 1) else false

    let solve (puzzle: int[]) =
        let p = Array.copy puzzle
        backtrack p nextValue 0 1 |> ignore
        p

    
