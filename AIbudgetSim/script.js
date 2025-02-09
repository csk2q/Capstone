document.getElementById("budgetForm").addEventListener("submit", async function(event) {
    event.preventDefault(); // Prevents page reload
 
    //  capture and send all the new input values to the backend.

    // Capture user input from all fields
    const budgetData = {
        income: document.getElementById("income").value,
        housing: document.getElementById("housing").value,
        electric: document.getElementById("electric").value,
        internet: document.getElementById("internet").value,
        gas: document.getElementById("gas").value,
        water: document.getElementById("water").value,
        trash: document.getElementById("trash").value,
        food: document.getElementById("food").value,
        leisure: document.getElementById("leisure").value,
        savings: document.getElementById("savings").value,
        investments: document.getElementById("investments").value
    };

    try {
        const response = await fetch("http://localhost:8080/api/budget-advice", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(budgetData)
        });

        const data = await response.json();
        document.getElementById("advice").innerText = data.advice;
    } catch (error) {
        console.error("Error fetching AI advice:", error);
        document.getElementById("advice").innerText = "Error fetching AI advice.";
    }
});
