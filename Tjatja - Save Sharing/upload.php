<?php    
    if(isset($_POST['name']) && isset($_POST['data'])){
        file_put_contents($_POST['name'].".txt", $_POST['data']);      
        echo "uploaded.";
    }else{
        echo "invalid file uploaded.";
    }  
?>