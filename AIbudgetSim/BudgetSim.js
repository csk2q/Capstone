import { useState } from "react";

function BudgetSim() {
  const [budget, setBudget] = useState({
    income: "",
    housing: "",
    electric: "",
    internet: "",
    gas: "",
    water: "",
    trash: "",
    food: "",
    leisure: "",
    savings: "",
    investments: ""
  });

  const [aiResponse, setAiResponse] = useState("");

  const handleChange = (e) => {
    setBudget({ ...budget, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    const response = await fetch("http://localhost:5000/api/budget-advice", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(budget)
    });
    const data = await response.json();
    setAiResponse(data.advice);
  };

  return (
    <div className="container">
      <h2>Budget Planner</h2>
      <p>Enter your monthly financial details and get AI-driven budget recommendations.</p>
      <form onSubmit={handleSubmit}>
        {Object.keys(budget).map((key) => (
          <div key={key}>
            <label>{key.charAt(0).toUpperCase() + key.slice(1)}</label>
            <input type="number" name={key} value={budget[key]} onChange={handleChange} required />
          </div>
        ))}
        <button type="submit">Get AI Advice</button>
      </form>
      <div id="adviceContainer">
        <h3>AI Budget Advice:</h3>
        <p>{aiResponse || "Your AI recommendations will appear here."}</p>
      </div>
    </div>
  );
}

export default BudgetSim;
