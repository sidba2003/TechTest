export default function UserDataComponent(props){
    async function deleteUser(id){
        try {
            const response = await fetch(`/api/users/${id}`, {
                method: "DELETE",
            });

            if (!response.ok) throw new Error("Failed to delete user");

            // delete the user from the list
            props.setUsers(users => users.filter(
                    user => user.id != id
                )
            );

            console.log("deleted the user");
        } catch (err) {
            console.error("Error deleting user:", err);
        }        
    }

    // id can be accessed using props.id.toString() (not sure about toString())
    return (
        <>
            <span className="user-email">{props.email}</span>
            <span className="user-surname">{props.surname}</span>
            <span className="user-forname">{props.forename}</span>
            <span className="user-dateOfBirth">{props.dateOfBirth.split("T")[0]}</span>
            <span className="user-isActive">{props.isActive.toString()}</span>
            <div className="user-options-class">
                <button onClick={() => deleteUser(props.id)} className="delete-option-class">
                    Delete
                </button>
                <button onClick={() => props.enableEditPopup(props.id)} className="edit-option-class">
                    Edit
                </button>
            </div>
        </>
    )
}