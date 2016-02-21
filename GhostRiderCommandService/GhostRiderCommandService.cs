using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.VoiceCommands;
using Windows.Storage;

namespace GhostRider.VoiceCommands {
    public sealed class GhostRiderCommandService : IBackgroundTask {









        private VoiceCommandServiceConnection voiceServiceConnection;
        public async void Run(IBackgroundTaskInstance taskInstance) {
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();
            var triggerDetails = taskInstance.TriggerDetails as AppServiceTriggerDetails;
            try {
                if (triggerDetails != null) {
                    voiceServiceConnection = VoiceCommandServiceConnection.FromAppServiceTriggerDetails(triggerDetails);
                    voiceServiceConnection.VoiceCommandCompleted += OnVoiceCommandComplete;
                    VoiceCommand voiceCommand = await voiceServiceConnection.GetVoiceCommandAsync();

                    var person = voiceCommand.Properties["person"][0];

                    var userMessage = new VoiceCommandUserMessage();
                    userMessage.DisplayMessage = "Which workflow would you like to trigger?";
                    userMessage.SpokenMessage = "Which workflow would you like to trigger?";
                    var workflowTiles = new List<VoiceCommandContentTile>() {
                        new VoiceCommandContentTile() {
                            ContentTileType = VoiceCommandContentTileType.TitleWith68x68IconAndText,
                            Image = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Images/GreyTile.scale-100.png")),
                            Title = "Create New Page",
                            AppLaunchArgument = $"person={person}",
                            TextLine1 = "Creates a new basic page"
                        },
                        new VoiceCommandContentTile() {
                            ContentTileType = VoiceCommandContentTileType.TitleWith68x68IconAndText,
                            Image = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Images/GreyTile.scale-100.png")),
                            Title = "Conquer the World",
                            AppLaunchArgument = $"person={person}",
                            TextLine1 = "Destroys all of your enemies"
                        },
                    };

                    var response = VoiceCommandResponse.CreateResponse(userMessage, workflowTiles);
                    await this.voiceServiceConnection.ReportSuccessAsync(response);
                    //await DetermineAction(voiceCommand);
                }

            }
            finally {
                deferral.Complete();
            }
        }

        private async Task DetermineAction(VoiceCommand voiceCommand) {
            switch (voiceCommand.CommandName) {
                case "showNotesForPerson":
                    var person = voiceCommand.Properties["person"][0];
                    await GetNotesForPerson(person);
                    break;
                default:
                    break;
            }
        }

        private async Task GetNotesForPerson(string person) {
            var userMessage = new VoiceCommandUserMessage();
            userMessage.DisplayMessage = "Here are your notes.";
            userMessage.SpokenMessage = "Tom is a bastard man";
            var personTiles = new List<VoiceCommandContentTile>();
            var personTile = new VoiceCommandContentTile() {
                ContentTileType = VoiceCommandContentTileType.TitleWith68x68IconAndText,
                Image = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Images/GreyTile.scale-100.png")),
                AppLaunchArgument = $"person={person}",
                Title = "Tom",
                TextLine1 = "Because he is"
            };

            personTiles.Add(personTile);

            var response = VoiceCommandResponse.CreateResponse(userMessage, personTiles);
            await this.voiceServiceConnection.ReportSuccessAsync(response);
        }

        private void OnVoiceCommandComplete(VoiceCommandServiceConnection voiceService, VoiceCommandCompletedEventArgs args) {

        }

    }
}
