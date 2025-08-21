import { Link } from "react-router-dom";

export default function LinkButtonComponent({ pageLink, buttonText }) {
    return (
        <Link to={pageLink}>
            <button className="goto-link-button-class">
                {buttonText}
            </button>
        </Link>
    );
}