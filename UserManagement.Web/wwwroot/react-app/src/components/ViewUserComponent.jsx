import { useSelector } from "react-redux";

export default function ViewUserComponent() {
    const userData = useSelector(state => state.user.selectedUser);
    
    if (!userData) return <p>No user selected</p>;

    async function obtainUserOps() {
        const url = `/api/user-audits/${userData.id}`;
        try {
            const response = await fetch(url);
            if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
            }

            const result = await response.json();
            console.log(result);
        } catch (error) {
            console.error(error.message);
        } 
    }

    obtainUserOps()

    return (
        <div>
            <h1>{userData.id}</h1>
            <h2>{userData.forename} {userData.surname}</h2>
            <p>Email: {userData.email}</p>
            <p>DOB: {userData.dateOfBirth}</p>
            <p>Active: {userData.isActive.toString()}</p>
        </div>
    );
}