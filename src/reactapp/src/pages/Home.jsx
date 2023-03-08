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
            <h1>Welcome to Microsoft Assessments</h1>

            <h2 id="introduction">Introduction</h2>
            <p>This application allows you to run assessments against your Microsoft 365 and Entra Azure Active Directory. The assessments checks your tenant configuration and provides recommendations on improving the security posture.</p>

            <h2 id="prerequisites">Pre-Requisites</h2>
            <p>In order to run the assessment, this application requires read-only access to your tenant. Consent will be required from the administrator when first run against a tenant.</p>

            <h2 id="privacy">Security</h2>
            <p>This application runs locally on your device and does not upload and content to any servers.</p>

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