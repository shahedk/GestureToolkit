name: DoubleTap

validation state
    Touch state: TouchUp
    Touch limit: 1
    Touch step: 2 touches within 1 sec
    
condition
    Touch area: Rect 50x50 including last 1 touch within 1 sec

return
    Info:DoubleTap