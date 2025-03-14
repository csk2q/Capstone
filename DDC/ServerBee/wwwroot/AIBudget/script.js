document.getElementById("getAdvice").addEventListener("click", async function () {
    console.log("✅ Button Clicked! Sending API Request..."); // Debugging log

    try {
        let income = document.getElementById("income").value;
        let housing = document.getElementById("housing").value;
        let electric = document.getElementById("electric").value;
        let internet = document.getElementById("internet").value;
        let gas = document.getElementById("gas").value;
        let water = document.getElementById("water").value;
        let trash = document.getElementById("trash").value;
        let food = document.getElementById("food").value;
        let leisure = document.getElementById("leisure").value;
        let savings = document.getElementById("savings").value;
        let investments = document.getElementById("investments").value;

        let requestBody = {
            income, housing, electric, internet, gas, water, trash, food, leisure, savings, investments
        };

        console.log("📤 Sending Request to API with body:", requestBody); // Debugging log

        let response = await fetch("/api/budget-advice", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(requestBody)
        });

        console.log("✅ API Response Received:", response); // Debugging log

        if (!response.ok) {
            throw new Error(`HTTP error! Status: ${response.status}`);
        }

        let data = await response.json();
        console.log("📨 AI Advice Received:", data); // Debugging log

        document.getElementById("adviceBox").innerHTML = `<b>AI Budget Advice:</b> ${data.advice}`;
    } catch (error) {
        console.error("❌ Error fetching AI advice:", error);
        document.getElementById("adviceBox").innerHTML = "❌ Error fetching AI advice. Please try again.";
    }
});
