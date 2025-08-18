export default function UserDataComponent(props){
    function printporps(){
        console.log("props are", props);
    }

    // id can be accessed using props.id.toString() (not sure about toString())
    return (
        <>
            {printporps()}
            <span className="user-email">{props.email}</span>
            <span className="user-surname">{props.surname}</span>
            <span className="user-forname">{props.forename}</span>
            <span className="user-dateOfBirth">{props.dateOfBirth.split("T")[0]}</span>
            <span className="user-isActive">{props.isActive.toString()}</span>
        </>
    )
}