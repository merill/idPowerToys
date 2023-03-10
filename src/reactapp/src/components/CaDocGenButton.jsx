import { makeStyles, tokens, Button } from "@fluentui/react-components";
import * as React from "react";
import { CalendarMonthRegular, } from "@fluentui/react-icons";
import { Providers, ProviderState } from "@microsoft/mgt";
import { useState, useEffect } from 'react';
import { Spinner, SpinnerProps } from "@fluentui/react-components";
import { Alert } from "@fluentui/react-components/unstable";

const useStyles = makeStyles({
    wrapper: {
        marginTop: tokens.spacingVerticalM,
        columnGap: "15px",
        display: "flex",
    },
});

export const CaDocGenButton = ({ isManual, caPolicyJson, token }) => {
    const styles = useStyles();
    const [showProgress, setShowProgress] = useState(false);
    const [showErrorAlert, setShowErrorAlert] = useState(false);

    const dismissAlert = async (e) => {
        setShowErrorAlert(false);
    }

    const handleClick = async () => {
        setShowProgress(true);
        setShowErrorAlert(false);

        let policy = {
            conditionalAccessPolicyJson: caPolicyJson,
            isManual: isManual
        };

        let token = "";
        if (Providers.globalProvider.state === ProviderState.SignedIn) {
            token = await Providers.globalProvider.getAccessToken({ scopes: ['User.Read'] })
        }

        const options = {
            method: 'POST',
            headers: {
                'Content-type': 'application/json',
                'X-PowerPointGeneration-Token': token
            },
            body: JSON.stringify(policy)
        };


        fetch('/powerpoint', options)
            .then((response) => {
                if (response.ok) {
                    return response.blob();
                }
                return null;
            })
            .then((blob) => {
                if (blob === null) {
                    setShowErrorAlert(true);
                }
                else {
                    // 2. Create blob link to download
                    const url = window.URL.createObjectURL(new Blob([blob]));
                    const link = document.createElement('a');
                    link.href = url;
                    link.setAttribute('download', `Conditional Access Policy Documentation.pptx`);
                    // 3. Append to html page
                    document.body.appendChild(link);
                    // 4. Force download
                    link.click();
                    // 5. Clean up and remove the link
                    link.parentNode.removeChild(link);
                }
                setShowProgress(false);
            })
            .catch((error) => {
                setShowErrorAlert(true);
                setShowProgress(false);
            });        
    };

    return (
        <>
            <div className={styles.wrapper}>
                <Button appearance="primary" icon={<CalendarMonthRegular />}
                    onClick={handleClick}
                >
                    Generate documentation
                </Button> {showProgress && <Spinner label="Creating presenating..." size="small" />}
            </div>
            {showErrorAlert &&
                <div style={{ display: "flex", flexDirection: "column", gap: "10px", marginTop: "20px" }}>
                    <Alert intent="error" action="Dismiss" onClick={dismissAlert}>
                        The presentation could not be generated.
                    </Alert>
                </div>
            }
        </>
    );
};