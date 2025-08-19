import { useState } from 'react';

export default function AddUserPopupComponent(props) {
    const [selectedDate, setSelectedDate] = useState(null);

    async function handleFormSubmission(e) {
        e.preventDefault(); // stop page refresh

        const formData = new FormData(e.target);

        // Convert FormData into a JS object
        const data = Object.fromEntries(formData.entries());

        // Convert checkbox string ("on") to boolean
        data.isActive = formData.get("isActive") === "on";

        // Make sure date is stored from state
        data.dateOfBirth = selectedDate;

        console.log("Submitting:", data);

        try {
        const response = await fetch("/api/users", {
            method: "POST",
            headers: {
            "Content-Type": "application/json",
            },
            body: JSON.stringify(data),
        });

        if (!response.ok) throw new Error("Failed to save user");

        const result = await response.json();

        // Add the new user to your list
        props.setUsers(prevUsers => [...prevUsers, result]);

        // Close the popup
        props.togglePopup(false);

        console.log("Saved user:", result);

        props.togglePopup(false);
        } catch (err) {
        console.error("Error saving user:", err);
        }
    }

    return (
        <form onSubmit={handleFormSubmission} className="add-user-popup-class">
            <label>
                Enter Email
                <input
                    type="email"
                    pattern=".+@.+\..+"
                    required
                    name="email"
                />
            </label>
            <label>
                Show Surname
                <input
                    type="text"
                    required
                    name="surname"
                />
            </label>
            <label>
                Enter Forename
                <input
                    type="text"
                    required
                    name="forename"
                />
            </label>
            <label>
                Date of Birth
                <input
                type="date"
                value={selectedDate}
                onChange={(e) => setSelectedDate(e.target.value)}
                required
                name="dateOfBirth"
                />
            </label>
            <label>
                Is Active
                <input
                    type="checkbox"
                    name="isActive"
                />
            </label>
            <div className="popup-buttons-class">
                <button className="popup-button-class cancel-button"
                    type="button" 
                    onClick={() => props.togglePopup(prev => !prev)}
                > Cancel </button>

                <button className="popup-button-class submit-button"
                > Submit </button>
            </div>
        </form>
    )
}