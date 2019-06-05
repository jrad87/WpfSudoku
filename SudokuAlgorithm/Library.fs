namespace SudokuAlgorithm

module RecursiveBacktrackingSolver =
    let row index = index / 9
    let col index = index % 9
    let index r c = 9 * r + c     
    let blockRange n = 3 * (n / 3)
    let checkGroup value getValue = 
        let rec check iteration = 
            if iteration = 9 then true
            elif getValue iteration = value then false
            else check (iteration + 1)
        check 0
    let checkRow (arr: int[]) row value = checkGroup value (fun col -> arr.[index row col])     
    let checkCol (arr: int[]) col value = checkGroup value (fun row -> arr.[index row col])
    let checkSector (arr: int[]) row col value = checkGroup value (fun count -> 
        arr.[index ((blockRange row) + count / 3) ((blockRange col) + (count % 3))])
    let isValidPlacement arr r c value = 
            checkRow arr r value &&
            checkCol arr c value &&
            checkSector arr r c value            
    let rec backtrack (grid: int[]) i value =    
        if i = 81 then true
        elif grid.[i] <> 0 then backtrack grid (i + 1) 1
        else
            if isValidPlacement grid (row i) (col i) value 
            then
                grid.[i] <- value
                if backtrack grid (i + 1) 1 then true else grid.[i] <- 0; nextValue grid i value
            else nextValue grid i value
    and nextValue grid i value =
        if (value + 1) < 10 then backtrack grid i (value + 1) else false
    let solve (puzzle: int[]) =
        let p = Array.copy puzzle
        backtrack p 0 1 |> ignore
        p 