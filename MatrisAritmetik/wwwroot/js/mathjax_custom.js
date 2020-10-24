function updateMathContent() {
    try
    {
        var math = document.getElementById("matris_table");
        MathJax.Hub.Queue(["Typeset", MathJax.Hub, math]);
        //console.log("rerendering math...");
    }
    catch(err) {
        console.log(err.message);
    }
} export { updateMathContent }