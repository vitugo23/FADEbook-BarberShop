import Link from "next/link";
import { Navigation } from "@/components/Navigation";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";

export default function Home() {
  return (
    <div className="min-h-screen flex flex-col">
      <Navigation />
      <main className="flex justify-center items-center min-h-screen flex-col">
        <section className="py-20 px-4">
          <div className="container mx-auto text-center">
            <h1 className="text-5xl font-bold mb-6">Fadebook</h1>
            <p className="text-xl text-muted-foreground mb-8 max-w-2xl mx-auto">
              Book your appointment with our expert barbers and get the perfect cut every time.
            </p>
            <div className="flex gap-4 justify-center">
              <Link href="/book">
                <Button size="lg">Book Appointment</Button>
              </Link>
              <Link href="/barbers">
                <Button size="lg" variant="outline">View Barbers</Button>
              </Link>
            </div>
          </div>
        </section>

        <section className="py-16 px-4 bg-muted/50">
          <div className="container mx-auto">
            <h2 className="text-3xl font-bold text-center mb-12">Our Services</h2>
            <div className="grid md:grid-cols-3 gap-6">
              <Card>
                <CardHeader>
                  <CardTitle>Professional Haircuts</CardTitle>
                  <CardDescription>Expert cuts tailored to your style</CardDescription>
                </CardHeader>
                <CardContent>
                  <p className="text-muted-foreground">
                    Our skilled barbers provide precision cuts that match your personality and lifestyle.
                  </p>
                </CardContent>
              </Card>
              <Card>
                <CardHeader>
                  <CardTitle>Beard Grooming</CardTitle>
                  <CardDescription>Keep your beard looking sharp</CardDescription>
                </CardHeader>
                <CardContent>
                  <p className="text-muted-foreground">
                    Professional beard trimming and styling to complement your look.
                  </p>
                </CardContent>
              </Card>
              <Card>
                <CardHeader>
                  <CardTitle>Hot Towel Shave</CardTitle>
                  <CardDescription>Traditional barbershop experience</CardDescription>
                </CardHeader>
                <CardContent>
                  <p className="text-muted-foreground">
                    Relax with our classic hot towel shave service for the smoothest finish.
                  </p>
                </CardContent>
              </Card>
            </div>
          </div>
        </section>
      </main>
    </div>
  );
}
