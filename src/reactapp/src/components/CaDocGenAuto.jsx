import * as React from "react";
import { useState, useEffect } from 'react';
import {
    makeStyles,
    tokens,
    useId,
    Label,
    Textarea,
    Button
} from "@fluentui/react-components";
import {
    bundleIcon,
    CalendarMonthFilled,
    CalendarMonthRegular,
} from "@fluentui/react-icons";

import { Providers, ProviderState } from "@microsoft/mgt";
import { Login } from '@microsoft/mgt-react';

const useStyles = makeStyles({
    base: {
        display: "flex",
        flexDirection: "column",
        "& > label": {
            display: "block",
            marginBottom: tokens.spacingVerticalMNudge,
        },
    },
    textarea: {
        height: "200px",
    },
    wrapper: {
        marginTop: tokens.spacingVerticalM,
        columnGap: "15px",
        display: "flex",
    },
});

function useIsSignedIn(): [boolean] {
    const [isSignedIn, setIsSignedIn] = useState(false);

    useEffect(() => {
        const updateState = () => {
            const provider = Providers.globalProvider;
            setIsSignedIn(provider && provider.state === ProviderState.SignedIn);
        };

        Providers.onProviderUpdated(updateState);
        updateState();

        return () => {
            Providers.removeProviderUpdatedListener(updateState);
        }
    }, []);

    return [isSignedIn];
}

export const CaDocGenAuto = () => {
    const styles = useStyles();
    const [isSignedIn] = useIsSignedIn();

    const handleClick = async () => {

        let policy = {
            conditionalAccessPolicyJson: "",
            isManual: false
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


        fetch('/weatherforecast', options)
            .then((response) => response.blob())
            .then((blob) => {

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
            <div className={styles.base}>
                <p>Connect to your tenant and export your conditional access policies as a PowerPoint presentation.</p>
                
                <div className={styles.wrapper}>

                    {isSignedIn &&
                        <Button appearance="primary" icon={<CalendarMonthRegular />}
                            onClick={handleClick}
                        >
                            Generate documentation
                        </Button>
                    }
                    {!isSignedIn &&
                        <div>
                            <p>Please sign in to generate documentation.</p>
                            <Login />
                        </div>
                    }
                </div>
            </div>
        </>
    );
};



