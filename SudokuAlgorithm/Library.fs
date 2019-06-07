namespace SudokuAlgorithm
open System.Collections.Generic
module RecursiveBacktrackingSolver =
    let memoize (f: 'a -> 'b) = 
        let cache = new Dictionary<'a, 'b>()
        let memoizedF (input: 'a) =
            match cache.TryGetValue input with
            | true, x -> x
            | false, _ -> let returnV = f input in cache.Add(input, returnV); returnV
        memoizedF    
    let row index = index / 9  
    let col index = index % 9 
    let index = let _index (r, c) = 9 * r + c in memoize _index     
    let blockRange = let _blockRange n = 3 * (n / 3) in memoize _blockRange     
    let lookupShifter shift index = let x = index + shift in if x >= 81 then x - 81 else x
    let lookupShiftMaker shift = lookupShifter shift
    let rec _checkGroup iteration value getValue =      
        if iteration = 9 then true
        elif getValue iteration = value then false
        else _checkGroup (iteration + 1) value getValue 
    let checkGroup = _checkGroup 0 
    let checkRow (arr: int[]) row value = checkGroup value (fun col -> arr.[index (row, col)])      
    let checkCol (arr: int[]) col value = checkGroup value (fun row -> arr.[index (row, col)]) 
    let checkSector (arr: int[]) row col value = checkGroup value (fun count -> 
        arr.[index (((blockRange row) + count / 3), ((blockRange col) + (count % 3)))]) 
    let isValidPlacement arr r c value = 
            checkRow arr r value &&
            checkCol arr c value &&
            checkSector arr r c value             
    let rec backtrack (grid: int[]) i value shifter =    
        if i = 81 then true 
        elif grid.[shifter i] <> 0 then backtrack grid (i + 1) 1 shifter
        else
            if isValidPlacement grid (row <| shifter i) (col <| shifter i) value 
            then
                grid.[shifter i] <- value
                if backtrack grid (i + 1) 1 shifter then true else grid.[shifter i] <- 0; nextValue grid i value shifter
            else nextValue grid i value shifter 
    and nextValue grid i value shifter =
        if (value + 1) < 10 then backtrack grid i (value + 1) shifter else false 
    let solve (puzzle: int[]) =
        let returnP = Array.copy puzzle 
        backtrack returnP 0 1 (lookupShiftMaker 9) |> ignore 
        returnP