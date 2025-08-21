import { useState } from "react";

export default function EditUserPopupComponent(props){
    const [selectedDate, setSelectedDate] = useState(props.dateOfBirth.split("T")[0]);

    function checkValuesChanged(data){
        if (data.isActive === props.isActive &&
            data.dateOfBirth === props.dateOfBirth.split("T")[0] &&
            data.email === props.email &&
            data.forename === props.forename &&
            data.surname === props.surname
        ) return false

        return true
    }

    async function handleFormSubmission(e){
        e.preventDefault();
        
        const formData = new FormData(e.target);
        const data = Object.fromEntries(formData.entries());

        data.isActive = formData.get("isActive") === "on";
        data.dateOfBirth = selectedDate;

        console.log("props are", props)
        console.log("form data is", data)
        
        // checks if one or more values have been changed to before sending the PUT request
        // avoids unecessary logs in the backend
        const valuesChanged = checkValuesChanged(data);
        if (!valuesChanged) {
            alert("Change one or more values to submit the updated data");
            return;
        }

        try {
            const response = await fetch(`/api/users/${props.id}`, {
                method: "PUT",
                headers: {
                "Content-Type": "application/json",
                },
                body: JSON.stringify(data),
            });

            if (!response.ok) throw new Error("Failed to edit user");

            const result = await response.json();

            props.setUsers((prevUsers) =>
                prevUsers.map((u) => (u.id === result.id ? result : u))
            );

            // Close the popup
            props.togglePopup(false);

            console.log("Edited the user:", result);
        } catch (err) {
            console.error("Error editting user:", err);
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
                    defaultValue={props.email}
                />
            </label>
            <label>
                Show Surname
                <input
                    type="text"
                    required
                    name="surname"
                    defaultValue={props.surname}
                />
            </label>
            <label>
                Enter Forename
                <input
                    type="text"
                    required
                    name="forename"
                    defaultValue={props.forename}
                />
            </label>
            <label>
                Date of Birth
                <input
                    type="date"
                    defaultValue={selectedDate}
                    onChange={(e) => setSelectedDate(e.target.value)}
                    required
                    name="dateOfBirth"
                    max={new Date().toISOString().split("T")[0]}
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
    );
}