name: Lasso

validation state
    Touch state: TouchUp
    Touch limit: 1
    
condition
    Closed loop and
    Touch path bounding box: 200x200..1000x1000 and
    Touch path length: 600..100000 and 
    Enclosed area:5000..1000000

return
    Touch points