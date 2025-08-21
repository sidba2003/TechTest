import { Link } from "react-router-dom";
import { useDispatch } from "react-redux";
import { setSelectedUser } from "../../store";

export default function UserDataComponent(props){
    const dispatch = useDispatch();

    const handleClick = () => {
        dispatch(setSelectedUser(props));
    };

    async function deleteUser(id){
        // toggling the edit popup off and resetting the edit user id
        // this helps with an edge case if a user whose edit popup is enabled is tried to be deleted
        props.setDisplayEditUserPopup(false);
        props.setEditUserPopupId(-1);

        try {
            const response = await fetch(`/api/users/${id}`, {
                method: "DELETE",
            });

            if (!response.ok) throw new Error("Failed to delete user");

            // delete the user from the list
            props.setUsers(users => users.filter(
                    user => user.id !== id
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
            <div className="user-option-class">
                <button onClick={() => deleteUser(props.id)} className="delete-option-class">
                    Delete
                </button>
                <button onClick={() => props.enableEditPopup(props.id)} className="edit-option-class">
                    Edit
                </button>
            </div>
            <Link onClick={handleClick} to='view'><div className="user-history-class"><div className="user-history-text"><span>History</span></div></div></Link>
        </>
    )
}