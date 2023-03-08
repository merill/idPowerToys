import React from 'react';
import { useState } from 'react';

function Home() {
    const [caPolicyJson, setCaPolicyJson] = useState("");

    const handleClick = async () => {

        let policy = {
            conditionalAccessPolicyJson: caPolicyJson
        };

        const options = {
            method: 'POST',
            headers: {
                'Content-type': 'application/json'
            },
            body: JSON.stringify(policy)
        };

        fetch('weatherforecast', options)
            .then((response) => response.blob())
            .then((blob) => {

                // 2. Create blob link to download
                const url = window.URL.createObjectURL(new Blob([blob]));
                const link = document.createElement('a');
                link.href = url;
                link.setAttribute('download', `CA Policy.pptx`);
                // 3. Append to html page
                document.body.appendChild(link);
                // 4. Force download
                link.click();
                // 5. Clean up and remove the link
                link.parentNode.removeChild(link);

            })
            .catch((error) => {
                error.json().then((json) => {
                    this.setState({
                        errors: json,
                    });
                })
            });
    };

    return (
        <>
            <h1>What is idPowerToys?</h1>
            <p>idPowerToys is a collection of mini-apps that I've been building (planning to build) that are related to Microsoft Entra and Azure Active Directory. Please note this is a personal project and is not affilitiated or endorsed by Microsoft.</p>

            <h2 id="introduction">Conditional Access Documenter</h2>
            <p>Generate a PowerPoint presentation of your Conditional Access policies to help visualize and share.</p>

            <input
                type="text"
                id="message"
                name="message"
                value={caPolicyJson}
                onChange={(e) => setCaPolicyJson(e.target.value)}
            />

            <button onClick={handleClick}>Update</button>
        </>
    );
}

export default Home;